namespace BarberFlow.Application.Features.Auth.Exceptions;
public class InvalidRefreshTokenException : Exception
{
    public InvalidRefreshTokenException()
        : base("The refresh token is invalid or has expired.")
    {
    }
}
