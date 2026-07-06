using BarberFlow.Application.Features.Appointments.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace BarberFlow.Application.Features.Appointments.Queries
{
    public class GetBarberShopAppointmentsQuery : IRequest<List<AppointmentDto>>
    {
        public Guid BarberShopId { get; set; }
    }
}
