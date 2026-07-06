namespace BarberFlow.Application.Features.Auth.DTOs
{
    public class RequestOtpResponse
    {
        public bool Success { get; set; }
        public DateTime Expiration { get; set; }
    }
}
