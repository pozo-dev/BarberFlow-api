namespace BarberFlow.Application.Features.ServicePrice.Exceptions;
public class ServicePriceNotFoundException : Exception
{
    public ServicePriceNotFoundException()
        : base("One or more services do not have a valid price.")
    {
    }
}
