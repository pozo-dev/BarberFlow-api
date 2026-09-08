using BarberFlow.Application.Features.Auth.DTOs;
using MediatR;

namespace BarberFlow.Application.Features.Auth.Commands.RefreshUserToken;
public class RefreshTokenCommand: IRequest<AuthResponseDto>
{
    public string DeviceId { get; set; }

    public string RefreshToken { get; set; }
}
