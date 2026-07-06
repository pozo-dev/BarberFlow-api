namespace BarberFlow.Application.Features.Auth.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("Credenciales inválidas.") { }
    }
}
