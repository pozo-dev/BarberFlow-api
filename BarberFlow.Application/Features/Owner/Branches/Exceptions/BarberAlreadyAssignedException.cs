namespace BarberFlow.Application.Features.Branches.Exceptions;
public class BarberAlreadyAssignedException : Exception
{
    public BarberAlreadyAssignedException()
        : base("El barbero ya está asignado a esta sucursal.")
    {
    }
}
