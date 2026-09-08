namespace BarberFlow.Application.Features.Appointments.Exceptions;
public class AppointmentCannotBeCancelledException : Exception
{
    public AppointmentCannotBeCancelledException()
        : base("Appointment cannot be cancelled.")
    {
    }
}
