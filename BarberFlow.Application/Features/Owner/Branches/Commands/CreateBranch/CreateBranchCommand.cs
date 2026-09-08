using MediatR;

namespace BarberFlow.Application.Features.Branches.Commands.CreateBranch;
public class CreateBranchCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int LocationSearchId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}
