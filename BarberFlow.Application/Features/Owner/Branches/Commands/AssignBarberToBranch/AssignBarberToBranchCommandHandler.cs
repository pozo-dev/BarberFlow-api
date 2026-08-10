using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Branches.Exceptions;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Branches.Commands.AssignBarberToBranch
{
    public class AssignBarberToBranchCommandHandler
        : IRequestHandler<AssignBarberToBranchCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IBarberAssignmentRepository _assignmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AssignBarberToBranchCommandHandler(
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
            AssignBarberToBranchCommand request,
            CancellationToken cancellationToken)
        {
            var currentProfileId = _currentUserService.ProfileId;

            if (currentProfileId == Guid.Empty)
                throw new CurrentProfileUnavailableException();

            var currentProfile = await _userProfileRepository.GetByIdAsync(
                currentProfileId,
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

            var barberProfile = await _userProfileRepository.GetByIdAsync(
                request.BarberProfileId,
                cancellationToken);

            if (barberProfile == null)
                throw new UserProfileNotFoundException();

            if (barberProfile.RoleId != 2)
                throw new ValidationException(
                    "El perfil seleccionado no corresponde a un barbero.");

            var alreadyAssigned = await _assignmentRepository.ExistsAsync(
                request.BranchId,
                request.BarberProfileId,
                cancellationToken);

            if (alreadyAssigned)
                throw new BarberAlreadyAssignedException();

            var assignment = BarberAssignment.Create(
                request.BranchId,
                request.BarberProfileId,
                request.IsPrimary);

            await _assignmentRepository.AddAsync(
                assignment,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}