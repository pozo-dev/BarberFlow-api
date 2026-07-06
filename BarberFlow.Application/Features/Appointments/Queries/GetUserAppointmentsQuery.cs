using BarberFlow.Application.Features.Appointments.DTOs;
using MediatR;

namespace BarberFlow.Application.Features.Appointments.Queries
{
    public class GetUserAppointmentsQuery : IRequest<List<AppointmentDto>>
    {
    }
}
