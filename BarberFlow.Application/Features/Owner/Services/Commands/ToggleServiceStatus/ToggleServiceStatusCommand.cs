using MediatR;

namespace BarberFlow.Application.Features.Services.Commands.ToggleServiceStatus;
public class ToggleServiceStatusCommand : IRequest
{
    public Guid Id { get; set; }
}
