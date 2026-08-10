using MediatR;
namespace BarberFlow.Application.Features.Collaborators.Queries.GetCollaboratorById
{
    public class GetCollaboratorByIdQuery : IRequest<CollaboratorDto> { public Guid Id { get; set; } }
}
