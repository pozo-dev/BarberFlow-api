using BarberFlow.Application.Features.Auth.DTOs;
using MediatR;

namespace BarberFlow.Application.Features.Auth.Commands.LoginWithOtp
{
    public class RequestOtpCommand : IRequest<OtpResponseDto>
    {
        public Guid UserId { get; set; }

        public Guid UserProfileId { get; set; }
    }
}
