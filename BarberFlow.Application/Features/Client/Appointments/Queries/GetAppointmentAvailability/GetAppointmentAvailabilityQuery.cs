using BarberFlow.Application.Common.Time;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Client.Appointments.Queries.GetAppointmentAvailability;

public sealed record GetAppointmentAvailabilityQuery(Guid BranchId, DateOnly Date, IReadOnlyCollection<Guid> ServiceIds, Guid? ProfessionalId) : IRequest<ClientAppointmentAvailabilityDto>;

public sealed class GetAppointmentAvailabilityHandler : IRequestHandler<GetAppointmentAvailabilityQuery, ClientAppointmentAvailabilityDto>
{
    private readonly IBranchRepository _branches;
    private readonly IServiceRepository _services;
    private readonly ICollaboratorRepository _collaborators;
    private readonly IAppointmentRepository _appointments;

    public GetAppointmentAvailabilityHandler(IBranchRepository branches, IServiceRepository services, ICollaboratorRepository collaborators, IAppointmentRepository appointments)
    {
        _branches = branches;
        _services = services;
        _collaborators = collaborators;
        _appointments = appointments;
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
        var booked = await _appointments.GetByBranchAndRangeAsync(branch.Id, dayStartUtc, nextDayStartUtc, cancellationToken);
        result.Slots.AddRange(AppointmentAvailabilityRules.GetAvailableSlots(request.Date, schedule.OpenTime, schedule.CloseTime, duration, candidates, booked, branch.TimeZoneId));
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
        IReadOnlyCollection<BarberFlow.Domain.Entities.Appointment> booked,
        string? timeZoneId = null)
    {
        var slots = new List<ClientAppointmentAvailabilitySlotDto>();
        for (var slot = date.ToDateTime(openTime); slot.Add(duration).TimeOfDay <= closeTime.ToTimeSpan(); slot = slot.AddMinutes(30))
        {
            var startAtUtc = BranchTimeZone.ToUtc(DateOnly.FromDateTime(slot), TimeOnly.FromDateTime(slot), timeZoneId);
            if (startAtUtc <= DateTimeOffset.UtcNow) continue;
            var end = slot.Add(duration);
            var endAtUtc = BranchTimeZone.ToUtc(DateOnly.FromDateTime(end), TimeOnly.FromDateTime(end), timeZoneId);
            var assignedProfessional = candidates.FirstOrDefault(candidate =>
                !booked.Any(appointment =>
                    appointment.CollaboratorId == candidate.Id &&
                    startAtUtc < appointment.EndDateTime &&
                    endAtUtc > appointment.StartDateTime));

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
