using MediatR;

namespace BarberFlow.Application.Features.Branches.Queries.GetMainBranch;
public class GetMainBranchQuery
    : IRequest<GetMainBranchResponseDto>
{
}
