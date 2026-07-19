namespace BarberFlow.Application.Features.BarberShops.Exceptions
{
    public class BarberShopNotFoundException : Exception
    {
        public BarberShopNotFoundException()
            : base("Barber shop not found.")
        {
        }
    }
}
