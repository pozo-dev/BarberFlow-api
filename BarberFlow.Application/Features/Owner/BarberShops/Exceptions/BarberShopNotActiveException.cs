namespace BarberFlow.Application.Features.BarberShops.Exceptions;
public class BarberShopNotActiveException : Exception
{
    public BarberShopNotActiveException()
        : base("Barber shop is not active.")
    {
    }
}
