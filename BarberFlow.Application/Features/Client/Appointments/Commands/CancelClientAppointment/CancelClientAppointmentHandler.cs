using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Client.Appointments.Commands.CancelClientAppointment;

public sealed class CancelClientAppointmentHandler : IRequestHandler<CancelClientAppointmentCommand>
{
    private readonly IClientAppointmentRepository _appointments;
    private readonly IAppointmentActivityRepository _appointmentActivities;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CancelClientAppointmentHandler(IClientAppointmentRepository appointments, IAppointmentActivityRepository appointmentActivities, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _appointments = appointments;
        _appointmentActivities = appointmentActivities;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CancelClientAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointments.GetOwnedByIdAsync(request.AppointmentId, _currentUser.UserId, cancellationToken)
            ?? throw new ClientAppointmentNotFoundException();

        if (!appointment.CanBeCancelled())
            throw new ClientAppointmentCannotBeCancelledException();

        appointment.Cancel();
        _appointmentActivities.Add(new AppointmentActivity(
            appointment.Id,
            AppointmentActivityType.Cancelled,
            _currentUser.ProfileId,
            _currentUser.RoleId));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
