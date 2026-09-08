using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Branches.Queries.GetMyBranches;
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
                LocationSearchId = x.LocationSearchId,
                LocationDisplayName = x.LocationSearch.DisplayName,
                PhoneNumber = x.PhoneNumber,
                IsActive = x.IsActive,
                IsMain = x.IsMain,
                MissingBookingRequirements = GetMissingBookingRequirements(x),
                CreatedAt = x.CreatedAt
            })
            .OrderBy(x => x.CreatedAt)
            .ToList();
    }

    private static IReadOnlyList<string> GetMissingBookingRequirements(
        BarberFlow.Domain.Entities.Branch branch)
    {
        var requirements = new List<string>();
        if (!branch.IsActive) requirements.Add("activation");
        if (branch.Schedules.Count != BarberFlow.Domain.Entities.Branch.WeeklyScheduleDays ||
            !branch.Schedules.Any(schedule => !schedule.IsClosed))
        {
            requirements.Add("schedules");
        }
        return requirements;
    }
}
