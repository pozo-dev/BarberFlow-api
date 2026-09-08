using MediatR;

namespace BarberFlow.Application.Features.Branches.Commands.RemoveBarberFromBranch;
public class RemoveBarberFromBranchCommand : IRequest
{
    public Guid Id { get; set; }

    public Guid BarberProfileId { get; set; }
}
