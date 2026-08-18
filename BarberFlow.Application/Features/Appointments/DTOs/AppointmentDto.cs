using BarberFlow.Domain.Enums;

namespace BarberFlow.Application.Features.Appointments.DTOs
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid BranchId { get; set; }
        public Guid? ProfessionalId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public List<Guid> ServiceIds { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}