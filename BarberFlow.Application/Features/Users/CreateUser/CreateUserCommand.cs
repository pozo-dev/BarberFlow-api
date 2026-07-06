using MediatR;

namespace BarberFlow.Application.Features.Users.CreateUser
{
    public class CreateUserCommand : IRequest<Guid>
    {
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
