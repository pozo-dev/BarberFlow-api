namespace BarberFlow.Application.Features.Services.Exceptions
{
    public class ServiceNotFoundException : Exception
    {
        public ServiceNotFoundException()
            : base("Service not found.")
        {
        }
    }
}
