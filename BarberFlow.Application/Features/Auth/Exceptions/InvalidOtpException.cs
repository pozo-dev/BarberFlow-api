namespace BarberFlow.Application.Features.Auth.Exceptions;
public class InvalidOtpException : Exception
{
    public InvalidOtpException() : base("Invalid OTP.") { }
}
