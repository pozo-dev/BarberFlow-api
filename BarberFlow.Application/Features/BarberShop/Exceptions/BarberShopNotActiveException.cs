namespace BarberFlow.Application.Features.BarberShop.Exceptions
{
    public class BarberShopNotActiveException : Exception
    {
        public BarberShopNotActiveException()
            : base("Barber shop is not active.")
        {
        }
    }
}
