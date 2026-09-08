namespace BarberFlow.Application.Features.Appointments.Exceptions;
public class AtLeastOneServiceRequiredException : Exception
{
    public AtLeastOneServiceRequiredException()
        : base("At least one service is required.")
    {
    }
}
