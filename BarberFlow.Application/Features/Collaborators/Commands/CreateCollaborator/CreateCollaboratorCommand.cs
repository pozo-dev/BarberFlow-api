using MediatR;
namespace BarberFlow.Application.Features.Collaborators.Commands.CreateCollaborator
{
    public class CreateCollaboratorCommand : IRequest<Guid>
    {
        public Guid BranchId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
