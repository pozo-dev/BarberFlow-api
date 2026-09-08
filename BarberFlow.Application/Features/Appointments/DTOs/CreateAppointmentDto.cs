namespace BarberFlow.Application.Features.Appointments.DTOs;
public class CreateAppointmentDto
{
    public Guid BranchId { get; set; }
    public Guid? ProfessionalId { get; set; }
    public DateTimeOffset StartDateTime { get; set; }
    public List<Guid> ServiceIds { get; set; } = new();
}
