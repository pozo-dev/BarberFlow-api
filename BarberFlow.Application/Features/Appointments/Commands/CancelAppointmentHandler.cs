using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Appointments.Exceptions;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Appointments.Commands
{
    public class CancelAppointmentHandler : IRequestHandler<CancelAppointmentCommand, bool>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CancelAppointmentHandler(
            IAppointmentRepository appointmentRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _appointmentRepository = appointmentRepository;
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

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
