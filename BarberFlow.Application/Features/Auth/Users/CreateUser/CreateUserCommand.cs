using MediatR;

namespace BarberFlow.Application.Features.Users.CreateUser
{
    public class CreateUserCommand : IRequest<CreateUserResponseDto>
    {
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
