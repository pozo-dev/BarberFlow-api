using MediatR;

namespace BarberFlow.Application.Features.Branches.Commands.ToggleBranchStatus
{
    public class ToggleBranchStatusCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}