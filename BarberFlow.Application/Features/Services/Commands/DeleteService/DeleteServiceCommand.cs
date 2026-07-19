using MediatR;

namespace BarberFlow.Application.Features.Services.Commands.DeleteService
{
    public class DeleteServiceCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
