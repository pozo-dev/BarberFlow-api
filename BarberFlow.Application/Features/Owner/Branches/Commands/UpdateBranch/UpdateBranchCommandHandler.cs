using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Branches.Exceptions;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Branches.Commands.UpdateBranch
{
    public class UpdateBranchCommandHandler
        : IRequestHandler<UpdateBranchCommand, UpdateBranchResponseDto>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBranchCommandHandler(
            IBranchRepository branchRepository,
            ICurrentUserService currentUserService,
            IUserProfileRepository userProfileRepository,
            IUnitOfWork unitOfWork)
        {
            _branchRepository = branchRepository;
            _currentUserService = currentUserService;
            _userProfileRepository = userProfileRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateBranchResponseDto> Handle(
            UpdateBranchCommand request,
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
                request.Id,
                cancellationToken);

            if (branch is null)
                throw new BranchNotFoundException();

            if (branch.BarberShopId != profile.BarberShopId)
                throw new ForbiddenAccessException();

            try
            {
                branch.Update(
                    request.Name,
                    request.Address,
                    request.City,
                    request.PhoneNumber);
            }
            catch (ArgumentException exception)
            {
                throw new ValidationException(exception.Message);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateBranchResponseDto { Id = branch.Id };
        }
    }
}
