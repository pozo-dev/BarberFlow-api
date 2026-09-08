using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Common.Time;
using BarberFlow.Application.Features.Appointments.Availability;
using BarberFlow.Application.Features.Client.Appointments;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Client.Appointments.Commands.RescheduleClientAppointment;

public sealed class RescheduleClientAppointmentHandler : IRequestHandler<RescheduleClientAppointmentCommand, Guid>
{
    private readonly IClientAppointmentRepository _clientAppointments;
    private readonly IAppointmentRepository _appointments;
    private readonly IAppointmentActivityRepository _appointmentActivities;
    private readonly IBranchRepository _branches;
    private readonly IServiceRepository _services;
    private readonly ICollaboratorRepository _collaborators;
    private readonly ICollaboratorAvailabilityRepository _availability;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public RescheduleClientAppointmentHandler(IClientAppointmentRepository clientAppointments, IAppointmentRepository appointments, IAppointmentActivityRepository appointmentActivities, IBranchRepository branches, IServiceRepository services, ICollaboratorRepository collaborators, ICurrentUserService currentUser, IUnitOfWork unitOfWork, ICollaboratorAvailabilityRepository availability)
    {
        _clientAppointments = clientAppointments;
        _appointments = appointments;
        _appointmentActivities = appointmentActivities;
        _branches = branches;
        _services = services;
        _collaborators = collaborators;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _availability = availability;
    }

    public async Task<Guid> Handle(RescheduleClientAppointmentCommand request, CancellationToken cancellationToken)
    {
        var original = await _clientAppointments.GetOwnedByIdAsync(request.AppointmentId, _currentUser.UserId, cancellationToken) ?? throw new ClientAppointmentNotFoundForRescheduleException();
        if (!original.CanBeCancelled()) throw new ClientAppointmentCannotBeRescheduledException();
        if (request.StartDateTime.Offset != TimeSpan.Zero || request.StartDateTime <= DateTimeOffset.UtcNow) throw new ClientAppointmentRescheduleConflictException();

        var branch = await _branches.GetByIdAsync(original.BranchId, cancellationToken);
        if (branch is null || !branch.IsActive || !branch.BarberShop.IsActive) throw new ClientAppointmentRescheduleConflictException();

        var serviceIds = original.AppointmentServices.Select(service => service.ServiceId).Distinct().ToList();
        var services = (await _services.GetByBarberShopIdAsync(branch.BarberShopId, cancellationToken)).Where(service => serviceIds.Contains(service.Id)).ToList();
        if (services.Count != serviceIds.Count || services.Any(service => !service.IsActive)) throw new ClientAppointmentRescheduleConflictException();

        var localStart = BranchTimeZone.ToBranchTime(request.StartDateTime, branch.TimeZoneId);
        var duration = TimeSpan.FromTicks(services.Sum(service => service.Duration.Ticks));
        var end = request.StartDateTime.Add(duration);
        var localEnd = BranchTimeZone.ToBranchTime(end, branch.TimeZoneId);
        var scheduleDay = BarberFlow.Domain.Enums.ScheduleDayExtensions.ToScheduleDay(localStart.DayOfWeek);
        var schedule = branch.Schedules.SingleOrDefault(item => item.DayOfWeek == scheduleDay);
        if (schedule is null || schedule.IsClosed || localStart.TimeOfDay < schedule.OpenTime.ToTimeSpan() || localEnd.Date != localStart.Date || localEnd.TimeOfDay > schedule.CloseTime.ToTimeSpan()) throw new ClientAppointmentRescheduleConflictException();

        var candidates = await _collaborators.GetActiveByBranchIdAsync(branch.Id, cancellationToken);
        await using var mutation = await _availability.BeginMutationAsync(candidates.Select(x => x.Id).Append(original.CollaboratorId).Distinct().ToList(), cancellationToken);
        var availability = await ProfessionalAvailability.LoadAsync(_availability, candidates.Select(x => x.Id).ToList(), request.StartDateTime, end, cancellationToken, original.Id);
        var candidateIds = request.ProfessionalId.HasValue ? candidates.Where(item => item.Id == request.ProfessionalId.Value).Select(item => item.Id) : candidates.Select(item => item.Id);
        var professionalId = Guid.Empty;
        foreach (var candidateId in candidateIds)
        {
            if (availability.CanAttend(candidateId, request.StartDateTime, end, branch.TimeZoneId))
            {
                professionalId = candidateId;
                break;
            }
        }
        if (professionalId == Guid.Empty) throw new ClientAppointmentRescheduleConflictException();

        var replacement = new Appointment(_currentUser.UserId, branch.Id, professionalId, request.StartDateTime, end);
        replacement.AddServices(services, services.ToDictionary(service => service.Id, service => service.Price));
        original.RescheduleTo(replacement.Id);
        _appointments.Add(replacement);
        _appointmentActivities.Add(new AppointmentActivity(
            original.Id,
            AppointmentActivityType.Rescheduled,
            _currentUser.ProfileId,
            _currentUser.RoleId,
            previousStartAtUtc: original.StartDateTime,
            newStartAtUtc: replacement.StartDateTime,
            relatedAppointmentId: replacement.Id));
        _appointmentActivities.Add(new AppointmentActivity(
            replacement.Id,
            AppointmentActivityType.Created,
            _currentUser.ProfileId,
            _currentUser.RoleId,
            newStartAtUtc: replacement.StartDateTime,
            relatedAppointmentId: original.Id));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await mutation.CommitAsync(cancellationToken);
        return replacement.Id;
    }
}
