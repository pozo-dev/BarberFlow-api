using BarberFlow.Application.Features.Appointments.DTOs;
using MediatR;

namespace BarberFlow.Application.Features.Appointments.Commands
{
    public class CreateAppointmentCommand : IRequest<AppointmentDto>
    {
        public CreateAppointmentDto Appointment { get; set; }
    }
}
