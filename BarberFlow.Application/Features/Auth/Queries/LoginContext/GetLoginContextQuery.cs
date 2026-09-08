using BarberFlow.Application.Features.Auth.DTOs;
using MediatR;

namespace BarberFlow.Application.Features.Auth.Queries.LoginContext;
public class GetLoginContextQuery : IRequest<LoginContextResponseDto>
{
    public string PhoneNumber { get; set; } = default!;
}
