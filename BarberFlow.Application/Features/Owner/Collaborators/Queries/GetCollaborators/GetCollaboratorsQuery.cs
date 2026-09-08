using MediatR;
namespace BarberFlow.Application.Features.Collaborators.Queries.GetCollaborators;
public class GetCollaboratorsQuery : IRequest<List<CollaboratorDto>> { }
