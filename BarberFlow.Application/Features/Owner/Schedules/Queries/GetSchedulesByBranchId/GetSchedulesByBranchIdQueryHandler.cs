using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Branches.Exceptions;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Schedules.Queries.GetSchedulesByBranchId
{
    public class GetSchedulesByBranchIdQueryHandler
        : IRequestHandler<GetSchedulesByBranchIdQuery, List<ScheduleDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IBranchRepository _branchRepository;

        public GetSchedulesByBranchIdQueryHandler(
            ICurrentUserService currentUserService,
            IUserProfileRepository userProfileRepository,
            IBranchRepository branchRepository)
        {
            _currentUserService = currentUserService;
            _userProfileRepository = userProfileRepository;
            _branchRepository = branchRepository;
        }

        public async Task<List<ScheduleDto>> Handle(
            GetSchedulesByBranchIdQuery request,
            CancellationToken cancellationToken)
        {
            var profileId = _currentUserService.ProfileId;

            if (profileId == Guid.Empty)
                throw new CurrentProfileUnavailableException();

            var profile = await _userProfileRepository.GetByIdAsync(
                profileId,
                cancellationToken);

            if (profile is null)
                throw new UserProfileNotFoundException();

            var branch = await _branchRepository.GetByIdAsync(
                request.BranchId,
                cancellationToken);

            if (branch is null)
                throw new BranchNotFoundException();

            if (branch.BarberShopId != profile.BarberShopId)
                throw new ForbiddenAccessException();

            return branch.Schedules
                .OrderBy(x => x.DayOfWeek)
                .Select(x => new ScheduleDto
                {
                    ScheduleId = x.Id,
                    DayOfWeek = x.DayOfWeek,
                    OpenTime = x.OpenTime.ToTimeSpan(),
                    CloseTime = x.CloseTime.ToTimeSpan(),
                    IsClosed = x.IsClosed
                })
                .ToList();
        }
    }
}
