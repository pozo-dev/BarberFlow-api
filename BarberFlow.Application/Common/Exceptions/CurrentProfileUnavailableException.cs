namespace BarberFlow.Application.Common.Exceptions;
public class CurrentProfileUnavailableException : Exception
{
    public CurrentProfileUnavailableException()
        : base("No se pudo obtener el perfil activo del usuario autenticado.")
    {
    }
}
