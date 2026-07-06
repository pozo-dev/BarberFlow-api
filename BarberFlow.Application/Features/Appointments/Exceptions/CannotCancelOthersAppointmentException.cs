namespace BarberFlow.Application.Features.Appointments.Exceptions
{
    public class CannotCancelOthersAppointmentException : Exception
    {
        public CannotCancelOthersAppointmentException()
            : base("You cannot cancel this appointment.")
        {
        }
    }
}
