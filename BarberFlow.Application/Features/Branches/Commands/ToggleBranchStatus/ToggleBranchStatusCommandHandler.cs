using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Branches.Exceptions;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Branches.Commands.ToggleBranchStatus
{
    public class ToggleBranchStatusCommandHandler
        : IRequestHandler<ToggleBranchStatusCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ToggleBranchStatusCommandHandler(
            ICurrentUserService currentUserService,
            IUserProfileRepository userProfileRepository,
            IBranchRepository branchRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _userProfileRepository = userProfileRepository;
            _branchRepository = branchRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            ToggleBranchStatusCommand request,
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

            var branch = await _branchRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (branch == null)
                throw new BranchNotFoundException();

            if (branch.BarberShopId != profile.BarberShopId)
                throw new ForbiddenAccessException();

            if (branch.IsActive)
                branch.Deactivate();
            else
                branch.Activate();

            _branchRepository.Update(branch);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}