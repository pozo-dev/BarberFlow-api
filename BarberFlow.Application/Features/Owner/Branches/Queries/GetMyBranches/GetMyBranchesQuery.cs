using MediatR;

namespace BarberFlow.Application.Features.Branches.Queries.GetMyBranches
{
    public class GetMyBranchesQuery : IRequest<List<BranchDto>>
    {
    }
}