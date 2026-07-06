namespace BarberFlow.Application.Features.BarberShop.Exceptions
{
    public class BarberShopNotFoundException : Exception
    {
        public BarberShopNotFoundException()
            : base("Barber shop not found.")
        {
        }
    }
}
