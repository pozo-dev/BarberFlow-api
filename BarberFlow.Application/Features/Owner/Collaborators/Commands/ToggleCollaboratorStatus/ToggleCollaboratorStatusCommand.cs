using MediatR;
namespace BarberFlow.Application.Features.Collaborators.Commands.ToggleCollaboratorStatus
{
    public class ToggleCollaboratorStatusCommand : IRequest { public Guid Id { get; set; } }
}
