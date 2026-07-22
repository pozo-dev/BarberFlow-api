using MediatR;

namespace BarberFlow.Application.Features.Branches.Commands.RemoveBarberFromBranch
{
    public class RemoveBarberFromBranchCommand : IRequest
    {
        public Guid BranchId { get; set; }

        public Guid BarberProfileId { get; set; }
    }
}
