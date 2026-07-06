using BarberFlow.Application.Features.Appointments.DTOs;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Appointments.Queries
{
    public class GetBarberShopAppointmentsHandler : IRequestHandler<GetBarberShopAppointmentsQuery, List<AppointmentDto>>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public GetBarberShopAppointmentsHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<AppointmentDto>> Handle(GetBarberShopAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetByBarberShopIdAsync(request.BarberShopId, cancellationToken);
            return appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                BarberShopId = a.BarberShopId,
                UserId = a.UserId,
                StartDateTime = a.StartDateTime,
                EndDateTime = a.EndDateTime,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            }).ToList();
        }
    }
}
