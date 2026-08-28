using BarberFlow.Application.Common.Time;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Client.Appointments.Queries.GetAppointmentAvailability;

public sealed record GetAppointmentAvailabilityCalendarQuery(
    Guid BranchId,
    DateOnly From,
    DateOnly To,
    IReadOnlyCollection<Guid> ServiceIds,
    Guid? ProfessionalId) : IRequest<ClientAppointmentAvailabilityCalendarDto>;

public sealed class GetAppointmentAvailabilityCalendarHandler : IRequestHandler<GetAppointmentAvailabilityCalendarQuery, ClientAppointmentAvailabilityCalendarDto>
{
    private readonly IBranchRepository _branches;
    private readonly IServiceRepository _services;
    private readonly ICollaboratorRepository _collaborators;
    private readonly IAppointmentRepository _appointments;

    public GetAppointmentAvailabilityCalendarHandler(IBranchRepository branches, IServiceRepository services, ICollaboratorRepository collaborators, IAppointmentRepository appointments)
    {
        _branches = branches;
        _services = services;
        _collaborators = collaborators;
        _appointments = appointments;
    }

    public async Task<ClientAppointmentAvailabilityCalendarDto> Handle(GetAppointmentAvailabilityCalendarQuery request, CancellationToken cancellationToken)
    {
        var result = new ClientAppointmentAvailabilityCalendarDto();
        var branch = await _branches.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null || !branch.IsActive || request.ServiceIds.Count == 0) return result;

        var requestedServiceIds = request.ServiceIds.Distinct().ToHashSet();
        var services = (await _services.GetByBarberShopIdAsync(branch.BarberShopId, cancellationToken))
            .Where(service => requestedServiceIds.Contains(service.Id) && service.IsActive)
            .ToList();
        if (services.Count != requestedServiceIds.Count) return result;

        var professionals = await _collaborators.GetActiveByBranchIdAsync(branch.Id, cancellationToken);
        result.HasActiveProfessionals = professionals.Count > 0;
        var candidates = professionals
            .Where(professional => !request.ProfessionalId.HasValue || professional.Id == request.ProfessionalId.Value)
            .ToList();
        if (candidates.Count == 0) return result;

        var fromUtc = BranchTimeZone.ToUtc(request.From, branch.TimeZoneId);
        var toUtc = BranchTimeZone.ToUtc(request.To.AddDays(1), branch.TimeZoneId);
        var booked = await _appointments.GetByBranchAndRangeAsync(branch.Id, fromUtc, toUtc, cancellationToken);
        var duration = TimeSpan.FromTicks(services.Sum(service => service.Duration.Ticks));
        for (var date = request.From; date <= request.To; date = date.AddDays(1))
        {
            var schedule = branch.Schedules.SingleOrDefault(item => item.DayOfWeek == AppointmentAvailabilityRules.ScheduleDayOfWeek(date));
            if (schedule is null || schedule.IsClosed) continue;
            if (AppointmentAvailabilityRules.GetAvailableSlots(date, schedule.OpenTime, schedule.CloseTime, duration, candidates, booked, branch.TimeZoneId).Count > 0)
                result.AvailableDates.Add(date);
        }
        return result;
    }
}

public sealed class ClientAppointmentAvailabilityCalendarDto
{
    public bool HasActiveProfessionals { get; set; }
    public List<DateOnly> AvailableDates { get; } = [];
}
