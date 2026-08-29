using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Owner.Appointments.Commands.UpdateOwnerAppointmentStatus;

public sealed class UpdateOwnerAppointmentStatusHandler : IRequestHandler<UpdateOwnerAppointmentStatusCommand>
{
    private readonly ICurrentUserService _currentUser; private readonly IUserProfileRepository _profiles; private readonly IOwnerAppointmentRepository _appointments; private readonly IUnitOfWork _unitOfWork;
    public UpdateOwnerAppointmentStatusHandler(ICurrentUserService currentUser, IUserProfileRepository profiles, IOwnerAppointmentRepository appointments, IUnitOfWork unitOfWork) => (_currentUser, _profiles, _appointments, _unitOfWork) = (currentUser, profiles, appointments, unitOfWork);
    public async Task Handle(UpdateOwnerAppointmentStatusCommand request, CancellationToken ct)
    {
        if (_currentUser.ProfileId == Guid.Empty) throw new CurrentProfileUnavailableException();
        var profile = await _profiles.GetByIdAsync(_currentUser.ProfileId, ct); if (profile?.BarberShopId is null) throw new ForbiddenAccessException();
        var appointment = await _appointments.GetOwnedByIdAsync(request.AppointmentId, profile.BarberShopId.Value, ct) ?? throw new OwnerAppointmentNotFoundException();
        if (appointment.Status != AppointmentStatus.Scheduled || appointment.StartDateTime > DateTimeOffset.UtcNow) throw new OwnerAppointmentCannotBeUpdatedException();
        if (request.Action == OwnerAppointmentAction.NoShow && appointment.StartDateTime.AddMinutes(15) > DateTimeOffset.UtcNow) throw new OwnerAppointmentCannotBeUpdatedException();
        if (request.Action == OwnerAppointmentAction.Complete) appointment.Completed(); else appointment.NoShow();
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
