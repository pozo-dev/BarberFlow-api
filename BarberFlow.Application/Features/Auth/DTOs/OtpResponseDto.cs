namespace BarberFlow.Application.Features.Auth.DTOs;
public class OtpResponseDto
{
    public Guid OtpId { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
