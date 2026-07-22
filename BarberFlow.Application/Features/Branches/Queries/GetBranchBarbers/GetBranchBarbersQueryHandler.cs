using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Branches.Queries.GetBranchBarbers
{
    public class GetBranchBarbersQueryHandler
        : IRequestHandler<GetBranchBarbersQuery, List<BranchBarberDto>>
    {
        private readonly IBarberAssignmentRepository _assignmentRepository;

        public GetBranchBarbersQueryHandler(
            IBarberAssignmentRepository assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public async Task<List<BranchBarberDto>> Handle(
            GetBranchBarbersQuery request,
            CancellationToken cancellationToken)
        {
            var assignments = await _assignmentRepository.GetByBranchIdAsync(
                request.BranchId,
                cancellationToken);

            return assignments
                .Select(x => new BranchBarberDto
                {
                    BarberProfileId = x.BarberProfileId,
                    UserId = x.BarberProfile.UserId,
                    PhoneNumber = x.BarberProfile.User.PhoneNumber,
                    IsPrimary = x.IsPrimary
                })
                .ToList();
        }
    }
}