using MediatR;
namespace BarberFlow.Application.Features.Collaborators.Commands.UpdateCollaborator
{
    public class UpdateCollaboratorCommand : IRequest
    {
        public Guid Id { get; set; }
        public Guid BranchId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
