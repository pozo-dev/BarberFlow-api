using BarberFlow.Domain.Enums;

namespace BarberFlow.Application.Features.Appointments.DTOs
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid BarberShopId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public List<Guid> ServiceIds { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
