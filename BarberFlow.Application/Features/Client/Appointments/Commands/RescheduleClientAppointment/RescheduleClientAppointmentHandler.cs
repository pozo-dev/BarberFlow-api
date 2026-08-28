using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Common.Time;
using BarberFlow.Application.Features.Client.Appointments;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Client.Appointments.Commands.RescheduleClientAppointment;

public sealed class RescheduleClientAppointmentHandler : IRequestHandler<RescheduleClientAppointmentCommand, Guid>
{
    private readonly IClientAppointmentRepository _clientAppointments;
    private readonly IAppointmentRepository _appointments;
    private readonly IBranchRepository _branches;
    private readonly IServiceRepository _services;
    private readonly ICollaboratorRepository _collaborators;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public RescheduleClientAppointmentHandler(IClientAppointmentRepository clientAppointments, IAppointmentRepository appointments, IBranchRepository branches, IServiceRepository services, ICollaboratorRepository collaborators, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _clientAppointments = clientAppointments;
        _appointments = appointments;
        _branches = branches;
        _services = services;
        _collaborators = collaborators;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
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
        var scheduleDay = (DayOfWeek)(((int)localStart.DayOfWeek + 6) % 7);
        var schedule = branch.Schedules.SingleOrDefault(item => item.DayOfWeek == scheduleDay);
        if (schedule is null || schedule.IsClosed || localStart.TimeOfDay < schedule.OpenTime.ToTimeSpan() || localEnd.Date != localStart.Date || localEnd.TimeOfDay > schedule.CloseTime.ToTimeSpan()) throw new ClientAppointmentRescheduleConflictException();

        var candidates = await _collaborators.GetActiveByBranchIdAsync(branch.Id, cancellationToken);
        var candidateIds = request.ProfessionalId.HasValue ? candidates.Where(item => item.Id == request.ProfessionalId.Value).Select(item => item.Id) : candidates.Select(item => item.Id);
        var professionalId = Guid.Empty;
        foreach (var candidateId in candidateIds)
        {
            if (!await _appointments.ExistsOverlappingAppointmentAsync(branch.Id, candidateId, request.StartDateTime, end, cancellationToken, original.Id))
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
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return replacement.Id;
    }
}
