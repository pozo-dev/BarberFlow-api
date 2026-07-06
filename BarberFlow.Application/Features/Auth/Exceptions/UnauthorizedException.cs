namespace BarberFlow.Application.Features.Auth.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
            : base("User is not authenticated.")
        {
        }
    }
}
