using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Appointments.Exceptions;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Enums;
using MediatR;

namespace BarberFlow.Application.Features.Appointments.Commands;
public class CancelAppointmentHandler : IRequestHandler<CancelAppointmentCommand, bool>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentActivityRepository _appointmentActivities;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CancelAppointmentHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentActivityRepository appointmentActivities,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentActivities = appointmentActivities;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (appointment == null)
            throw new AppointmentNotFoundException();

        if (appointment.UserId != userId)
            throw new CannotCancelOthersAppointmentException();

        if (!appointment.CanBeCancelled())
            throw new AppointmentCannotBeCancelledException();

        appointment.Cancel();
        _appointmentActivities.Add(new AppointmentActivity(
            appointment.Id,
            AppointmentActivityType.Cancelled,
            _currentUser.ProfileId,
            _currentUser.RoleId));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
