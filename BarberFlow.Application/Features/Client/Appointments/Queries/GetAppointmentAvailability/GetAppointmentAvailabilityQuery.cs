using BarberFlow.Application.Common.Time;
using BarberFlow.Application.Features.Appointments.Availability;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Client.Appointments.Queries.GetAppointmentAvailability;

public sealed record GetAppointmentAvailabilityQuery(Guid BranchId, DateOnly Date, IReadOnlyCollection<Guid> ServiceIds, Guid? ProfessionalId) : IRequest<ClientAppointmentAvailabilityDto>;

public sealed class GetAppointmentAvailabilityHandler : IRequestHandler<GetAppointmentAvailabilityQuery, ClientAppointmentAvailabilityDto>
{
    private readonly IBranchRepository _branches;
    private readonly IServiceRepository _services;
    private readonly ICollaboratorRepository _collaborators;
    private readonly ICollaboratorAvailabilityRepository _availability;

    public GetAppointmentAvailabilityHandler(IBranchRepository branches, IServiceRepository services, ICollaboratorRepository collaborators, ICollaboratorAvailabilityRepository availability)
    {
        _branches = branches;
        _services = services;
        _collaborators = collaborators;
        _availability = availability;
    }

    public async Task<ClientAppointmentAvailabilityDto> Handle(GetAppointmentAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var result = new ClientAppointmentAvailabilityDto { BranchId = request.BranchId, Date = request.Date };
        var branch = await _branches.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null || !branch.IsActive || request.ServiceIds.Count == 0) return result;
        var requestedServiceIds = request.ServiceIds.Distinct().ToHashSet();
        var services = (await _services.GetByBarberShopIdAsync(branch.BarberShopId, cancellationToken))
            .Where(x => requestedServiceIds.Contains(x.Id))
            .ToList();
        if (services.Count != requestedServiceIds.Count || services.Any(x => !x.IsActive)) return result;
        var schedule = branch.Schedules.SingleOrDefault(x => x.DayOfWeek == AppointmentAvailabilityRules.ScheduleDayOfWeek(request.Date));
        if (schedule is null || schedule.IsClosed) return result;
        var duration = TimeSpan.FromTicks(services.Sum(x => x.Duration.Ticks));
        var professionals = await _collaborators.GetActiveByBranchIdAsync(branch.Id, cancellationToken);
        var candidates = professionals
            .Where(x => !request.ProfessionalId.HasValue || x.Id == request.ProfessionalId.Value)
            .ToList();
        var dayStartUtc = BranchTimeZone.ToUtc(request.Date, branch.TimeZoneId);
        var nextDayStartUtc = BranchTimeZone.ToUtc(request.Date.AddDays(1), branch.TimeZoneId);
        var availability = await ProfessionalAvailability.LoadAsync(_availability, candidates.Select(x => x.Id).ToList(), dayStartUtc, nextDayStartUtc, cancellationToken);
        result.Slots.AddRange(AppointmentAvailabilityRules.GetAvailableSlots(request.Date, schedule.OpenTime, schedule.CloseTime, duration, candidates, availability, branch.TimeZoneId));
        return result;
    }
}

internal static class AppointmentAvailabilityRules
{
    internal static BarberFlow.Domain.Enums.ScheduleDay ScheduleDayOfWeek(DateOnly date) => BarberFlow.Domain.Enums.ScheduleDayExtensions.ToScheduleDay(date.DayOfWeek);

    internal static List<ClientAppointmentAvailabilitySlotDto> GetAvailableSlots(
        DateOnly date,
        TimeOnly openTime,
        TimeOnly closeTime,
        TimeSpan duration,
        IReadOnlyCollection<BarberFlow.Domain.Entities.Collaborator> candidates,
        ProfessionalAvailability availability,
        string timeZoneId)
    {
        var slots = new List<ClientAppointmentAvailabilitySlotDto>();
        if (duration <= TimeSpan.Zero || closeTime <= openTime) return slots;
        var closing = date.ToDateTime(closeTime);
        for (var slot = date.ToDateTime(openTime); slot.Add(duration) <= closing; slot = slot.AddMinutes(30))
        {
            var startAtUtc = BranchTimeZone.ToUtc(DateOnly.FromDateTime(slot), TimeOnly.FromDateTime(slot), timeZoneId);
            if (startAtUtc <= DateTimeOffset.UtcNow) continue;
            var end = slot.Add(duration);
            var endAtUtc = BranchTimeZone.ToUtc(DateOnly.FromDateTime(end), TimeOnly.FromDateTime(end), timeZoneId);
            var assignedProfessional = candidates.FirstOrDefault(candidate =>
                availability.CanAttend(candidate.Id, startAtUtc, endAtUtc, timeZoneId));

            if (assignedProfessional is not null)
                slots.Add(new ClientAppointmentAvailabilitySlotDto
                {
                    StartAtUtc = startAtUtc,
                    DisplayTime = TimeOnly.FromDateTime(slot),
                    ProfessionalId = assignedProfessional.Id,
                    ProfessionalName = assignedProfessional.FullName,
                });
        }
        return slots;
    }
}

public sealed class ClientAppointmentAvailabilityDto
{
    public Guid BranchId { get; init; }
    public DateOnly Date { get; init; }
    public List<ClientAppointmentAvailabilitySlotDto> Slots { get; init; } = new();
}

public sealed class ClientAppointmentAvailabilitySlotDto
{
    public DateTimeOffset StartAtUtc { get; init; }
    public TimeOnly DisplayTime { get; init; }
    public Guid ProfessionalId { get; init; }
    public string ProfessionalName { get; init; } = string.Empty;
}
