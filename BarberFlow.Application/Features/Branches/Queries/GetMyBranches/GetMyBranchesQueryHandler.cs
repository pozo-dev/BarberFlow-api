using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Branches.Queries.GetMyBranches
{
    public class GetMyBranchesQueryHandler
        : IRequestHandler<GetMyBranchesQuery, List<BranchDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IBranchRepository _branchRepository;

        public GetMyBranchesQueryHandler(
            ICurrentUserService currentUserService,
            IUserProfileRepository userProfileRepository,
            IBranchRepository branchRepository)
        {
            _currentUserService = currentUserService;
            _userProfileRepository = userProfileRepository;
            _branchRepository = branchRepository;
        }

        public async Task<List<BranchDto>> Handle(
            GetMyBranchesQuery request,
            CancellationToken cancellationToken)
        {
            var profileId = _currentUserService.ProfileId;

            if (profileId == Guid.Empty)
                throw new CurrentProfileUnavailableException();

            var profile = await _userProfileRepository.GetByIdAsync(
                profileId,
                cancellationToken);

            if (profile == null)
                throw new UserProfileNotFoundException();

            if (profile.BarberShopId == null)
                return [];

            var branches = await _branchRepository.GetByBarberShopIdAsync(
                profile.BarberShopId.Value,
                cancellationToken);

            return branches
                .Select(x => new BranchDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    PhoneNumber = x.PhoneNumber,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                })
                .ToList();
        }
    }
}