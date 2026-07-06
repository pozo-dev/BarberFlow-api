namespace BarberFlow.Application.Features.Appointments.DTOs
{
    public class CreateAppointmentDto
    {
        public Guid BarberShopId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public List<Guid> ServiceIds { get; set; }
    }
}
