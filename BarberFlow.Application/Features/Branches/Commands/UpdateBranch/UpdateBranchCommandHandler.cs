using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Branches.Exceptions;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Branches.Commands.UpdateBranch
{
    public class UpdateBranchCommandHandler
        : IRequestHandler<UpdateBranchCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBranchCommandHandler(
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
            UpdateBranchCommand request,
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

            branch.Update(
                request.Name,
                request.Address,
                request.PhoneNumber);

            _branchRepository.Update(branch);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}