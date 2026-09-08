namespace BarberFlow.Application.Features.Services.Exceptions;
public class ServiceNotActiveException : Exception
{
    public ServiceNotActiveException()
        : base("Service is not active.")
    {
    }
}
