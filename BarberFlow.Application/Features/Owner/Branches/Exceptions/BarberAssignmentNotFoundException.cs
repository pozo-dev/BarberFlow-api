namespace BarberFlow.Application.Features.Branches.Exceptions;
public class BarberAssignmentNotFoundException : Exception
{
    public BarberAssignmentNotFoundException()
        : base("La asignación del barbero no fue encontrada.")
    {
    }
}
