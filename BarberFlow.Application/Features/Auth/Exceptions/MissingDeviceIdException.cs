namespace BarberFlow.Application.Features.Auth.Exceptions
{
    public class MissingDeviceIdException : Exception
    {
        public MissingDeviceIdException()
            : base("DeviceId is required.")
        {
        }
    }
}
