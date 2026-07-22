using MediatR;

namespace BarberFlow.Application.Features.Branches.Queries.GetBranchBarbers
{
    public class GetBranchBarbersQuery
        : IRequest<List<BranchBarberDto>>
    {
        public Guid BranchId { get; set; }
    }
}