using MediatR;

namespace BarberFlow.Application.Features.Auth.Commands.Logout;
public sealed class LogoutCommand : IRequest
{
    public string RefreshToken { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
}
