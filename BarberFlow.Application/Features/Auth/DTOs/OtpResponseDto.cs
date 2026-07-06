namespace BarberFlow.Application.Features.Auth.DTOs
{
    public class OtpResponseDto
    {
        public string OtpCode { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
