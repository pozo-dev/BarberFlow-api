using BarberFlow.Application.Features.Auth.DTOs;
using MediatR;

namespace BarberFlow.Application.Features.Auth.Commands.VerifyOtp
{
    public class VerifyOtpCommand : IRequest<AuthResponseDto>
    {
        public Guid OtpId { get; set; }
        public string DeviceId { get; set; }
    }
}
