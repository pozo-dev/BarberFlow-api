using MediatR;

namespace BarberFlow.Application.Features.Appointments.Commands;
public class CancelAppointmentCommand : IRequest<bool>
{
    public Guid AppointmentId { get; set; }
}
