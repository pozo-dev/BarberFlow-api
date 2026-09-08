namespace BarberFlow.Application.Features.Branches.Exceptions;
public class DuplicateBranchNameException : Exception
{
    public DuplicateBranchNameException(string branchName)
        : base($"Ya existe una sucursal con el nombre '{branchName}'.")
    {
    }
}
