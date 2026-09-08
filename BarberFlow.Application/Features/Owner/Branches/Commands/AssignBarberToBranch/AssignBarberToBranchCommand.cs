using MediatR;

namespace BarberFlow.Application.Features.Branches.Commands.AssignBarberToBranch;
public class AssignBarberToBranchCommand : IRequest
{
    public Guid BranchId { get; set; }

    public Guid BarberProfileId { get; set; }

    public bool IsPrimary { get; set; }
}
