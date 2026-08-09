using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Branches.Queries.GetBranchBarbers
{
    public class GetBranchBarbersQueryHandler
        : IRequestHandler<GetBranchBarbersQuery, List<BranchBarberDto>>
    {
        private readonly IBarberAssignmentRepository _assignmentRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly BarberFlow.Application.Common.Interfaces.ICurrentUserService _currentUserService;

        public GetBranchBarbersQueryHandler(
            IBarberAssignmentRepository assignmentRepository,
            IBranchRepository branchRepository,
            BarberFlow.Application.Common.Interfaces.ICurrentUserService currentUserService)
        {
            _assignmentRepository = assignmentRepository;
            _branchRepository = branchRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<BranchBarberDto>> Handle(
            GetBranchBarbersQuery request,
            CancellationToken cancellationToken)
        {
            var branch = await _branchRepository.GetByIdAsync(request.BranchId, cancellationToken);

            if (branch is null || branch.BarberShop.OwnerUserId != _currentUserService.UserId)
                throw new BarberFlow.Application.Common.Exceptions.ForbiddenAccessException();

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
