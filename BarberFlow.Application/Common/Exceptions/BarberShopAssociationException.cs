namespace BarberFlow.Application.Common.Exceptions
{
    public class BarberShopAssociationException : Exception
    {
        public BarberShopAssociationException()
            : base("El usuario no tiene una barbería asociada.")
        {
        }
    }
}
