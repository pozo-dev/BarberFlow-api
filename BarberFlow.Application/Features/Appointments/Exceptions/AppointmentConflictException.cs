namespace BarberFlow.Application.Features.Appointments.Exceptions
{
    public class AppointmentConflictException : Exception
    {
        public AppointmentConflictException()
            : base("The selected time slot is already booked.")
        {
        }
    }
}
