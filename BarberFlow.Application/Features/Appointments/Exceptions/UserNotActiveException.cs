namespace BarberFlow.Application.Features.Appointments.Exceptions
{
    public class UserNotActiveException : Exception
    {
        public UserNotActiveException() : base("User is not active.") { }
    }
}
