using MediatR;

namespace BarberFlow.Application.Features.Branches.Queries.GetBranchById
{
    public class GetBranchByIdQuery
    : IRequest<BranchDetailResponseDto>
    {
        public Guid Id { get; set; }
    }
}
