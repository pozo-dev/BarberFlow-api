namespace BarberFlow.Application.Features.Branches.Exceptions
{
    public class BranchNotFoundException : Exception
    {
        public BranchNotFoundException()
            : base("La sucursal no fue encontrada.")
        {
        }
    }
}
