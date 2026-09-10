using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Owner.Appointments.Commands.UpdateOwnerAppointmentStatus;

public sealed class UpdateOwnerAppointmentStatusHandler : IRequestHandler<UpdateOwnerAppointmentStatusCommand>
{
    private readonly ICurrentUserService _currentUser; private readonly IUserProfileRepository _profiles; private readonly IOwnerAppointmentRepository _appointments; private readonly IAppointmentActivityRepository _appointmentActivities; private readonly IUnitOfWork _unitOfWork;
    public UpdateOwnerAppointmentStatusHandler(ICurrentUserService currentUser, IUserProfileRepository profiles, IOwnerAppointmentRepository appointments, IAppointmentActivityRepository appointmentActivities, IUnitOfWork unitOfWork) => (_currentUser, _profiles, _appointments, _appointmentActivities, _unitOfWork) = (currentUser, profiles, appointments, appointmentActivities, unitOfWork);
    public async Task Handle(UpdateOwnerAppointmentStatusCommand request, CancellationToken ct)
    {
        if (_currentUser.ProfileId == Guid.Empty) throw new CurrentProfileUnavailableException();
        var profile = await _profiles.GetByIdAsync(_currentUser.ProfileId, ct); if (profile?.BarberShopId is null) throw new ForbiddenAccessException();
        var appointment = await _appointments.GetOwnedByIdAsync(request.AppointmentId, profile.BarberShopId.Value, ct) ?? throw new OwnerAppointmentNotFoundException();
        var now = DateTimeOffset.UtcNow;
        if (appointment.Status != AppointmentStatus.Scheduled) throw new OwnerAppointmentCannotBeUpdatedException();
        if (request.Action == OwnerAppointmentAction.Complete && appointment.EndDateTime > now) throw new OwnerAppointmentCannotBeUpdatedException();
        if (request.Action == OwnerAppointmentAction.NoShow && appointment.StartDateTime.AddMinutes(15) > now) throw new OwnerAppointmentCannotBeUpdatedException();
        if (request.Action == OwnerAppointmentAction.Complete) appointment.Completed(); else appointment.NoShow();
        _appointmentActivities.Add(new AppointmentActivity(appointment.Id, request.Action == OwnerAppointmentAction.Complete ? AppointmentActivityType.Completed : AppointmentActivityType.NoShow, _currentUser.ProfileId, _currentUser.RoleId));
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
