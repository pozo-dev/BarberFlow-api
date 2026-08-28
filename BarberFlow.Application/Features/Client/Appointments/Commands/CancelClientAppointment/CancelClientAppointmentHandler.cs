using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Interfaces;
using MediatR;

namespace BarberFlow.Application.Features.Client.Appointments.Commands.CancelClientAppointment;

public sealed class CancelClientAppointmentHandler : IRequestHandler<CancelClientAppointmentCommand>
{
    private readonly IClientAppointmentRepository _appointments;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CancelClientAppointmentHandler(IClientAppointmentRepository appointments, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _appointments = appointments;
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
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
