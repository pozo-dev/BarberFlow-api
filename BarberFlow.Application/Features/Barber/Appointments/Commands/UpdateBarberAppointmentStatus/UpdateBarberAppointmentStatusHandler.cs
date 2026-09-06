using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Constants;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Barber.Appointments.Commands.UpdateBarberAppointmentStatus;

public sealed class UpdateBarberAppointmentStatusHandler : IRequestHandler<UpdateBarberAppointmentStatusCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserProfileRepository _profiles;
    private readonly IBarberAppointmentRepository _appointments;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBarberAppointmentStatusHandler(ICurrentUserService currentUser, IUserProfileRepository profiles, IBarberAppointmentRepository appointments, IUnitOfWork unitOfWork) =>
        (_currentUser, _profiles, _appointments, _unitOfWork) = (currentUser, profiles, appointments, unitOfWork);

    public async Task Handle(UpdateBarberAppointmentStatusCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.ProfileId == Guid.Empty) throw new CurrentProfileUnavailableException();
        var profile = await _profiles.GetByIdAsync(_currentUser.ProfileId, cancellationToken);
        if (profile?.RoleId != RoleIds.Barber || !profile.IsActive) throw new ForbiddenAccessException();

        var appointment = await _appointments.GetOwnedByIdAsync(request.AppointmentId, profile.Id, cancellationToken)
            ?? throw new BarberAppointmentNotFoundException();
        if (appointment.Status != AppointmentStatus.Scheduled)
            throw new BarberAppointmentCannotBeUpdatedException();

        var now = DateTimeOffset.UtcNow;
        if (request.Action == BarberAppointmentAction.Complete && appointment.EndDateTime > now)
            throw new BarberAppointmentCannotBeUpdatedException();
        if (request.Action == BarberAppointmentAction.NoShow && appointment.StartDateTime.AddMinutes(15) > now)
            throw new BarberAppointmentCannotBeUpdatedException();

        if (request.Action == BarberAppointmentAction.Complete) appointment.Completed();
        else appointment.NoShow();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
