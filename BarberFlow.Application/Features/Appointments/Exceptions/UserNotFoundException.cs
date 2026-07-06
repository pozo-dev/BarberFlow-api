namespace BarberFlow.Application.Features.Appointments.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("User not found.") { }
    }
}
