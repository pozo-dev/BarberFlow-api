namespace BarberFlow.Application.Features.Auth.DTOs
{
    public class OtpResponseDto
    {
        public Guid OtpId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
