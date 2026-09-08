namespace BarberFlow.Application.Features.Appointments.Exceptions;
public class AppointmentNotFoundException : Exception
{
    public AppointmentNotFoundException() : base("Appointment not found") { }
}
