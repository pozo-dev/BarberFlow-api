namespace BarberFlow.Application.Common.Exceptions
{
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException()
            : base("No tiene permisos para realizar esta acción.")
        {
        }
    }
}
