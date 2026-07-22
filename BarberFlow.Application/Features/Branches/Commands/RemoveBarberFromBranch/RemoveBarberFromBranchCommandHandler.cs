using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Branches.Exceptions;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Branches.Commands.RemoveBarberFromBranch
{
    public class RemoveBarberFromBranchCommandHandler
        : IRequestHandler<RemoveBarberFromBranchCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IBarberAssignmentRepository _assignmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveBarberFromBranchCommandHandler(
            ICurrentUserService currentUserService,
            IUserProfileRepository userProfileRepository,
            IBranchRepository branchRepository,
            IBarberAssignmentRepository assignmentRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _userProfileRepository = userProfileRepository;
            _branchRepository = branchRepository;
            _assignmentRepository = assignmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            RemoveBarberFromBranchCommand request,
            CancellationToken cancellationToken)
        {
            var profileId = _currentUserService.ProfileId;

            if (profileId == Guid.Empty)
                throw new CurrentProfileUnavailableException();

            var currentProfile = await _userProfileRepository.GetByIdAsync(
                profileId,
                cancellationToken);

            if (currentProfile == null)
                throw new UserProfileNotFoundException();

            var branch = await _branchRepository.GetByIdAsync(
                request.BranchId,
                cancellationToken);

            if (branch == null)
                throw new BranchNotFoundException();

            if (branch.BarberShopId != currentProfile.BarberShopId)
                throw new ForbiddenAccessException();

            var assignment = await _assignmentRepository.GetAsync(
                request.BranchId,
                request.BarberProfileId,
                cancellationToken);

            if (assignment == null)
                throw new BarberAssignmentNotFoundException();

            assignment.Deactivate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
