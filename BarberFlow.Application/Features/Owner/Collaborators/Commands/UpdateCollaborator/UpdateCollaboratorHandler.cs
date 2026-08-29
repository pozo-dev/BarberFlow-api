using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Common.Validation;
using BarberFlow.Application.Features.Branches.Exceptions;
using BarberFlow.Application.Features.Collaborators.Exceptions;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Collaborators.Commands.UpdateCollaborator
{
    public class UpdateCollaboratorHandler : IRequestHandler<UpdateCollaboratorCommand>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IUserProfileRepository _profiles;
        private readonly IBranchRepository _branches;
        private readonly ICollaboratorRepository _collaborators;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCollaboratorHandler(ICurrentUserService currentUser,
            IUserProfileRepository profiles,
            IBranchRepository branches,
            ICollaboratorRepository collaborators,
            IUnitOfWork unitOfWork)
            => (_currentUser, _profiles, _branches, _collaborators, _unitOfWork) =
                (currentUser, profiles, branches, collaborators, unitOfWork);

        public async Task Handle(UpdateCollaboratorCommand request, CancellationToken ct)
        {
            var profile = await GetProfileAsync(ct);
            var collaborator = await _collaborators.GetByIdAsync(request.Id, ct) ?? throw new CollaboratorNotFoundException();
            var branch = await _branches.GetByIdAsync(request.BranchId, ct) ?? throw new BranchNotFoundException();
            if (collaborator.Branch.BarberShopId != profile.BarberShopId || branch.BarberShopId != profile.BarberShopId)
                throw new ForbiddenAccessException();
            try
            {
                collaborator.Update(request.BranchId, request.FullName, PhoneNumberValidator.NormalizeAndValidate(request.PhoneNumber));
            }
            catch (ArgumentException e) 
            { 
                throw new ValidationException(e.Message);
            }
            await _unitOfWork.SaveChangesAsync(ct);
        }

        private async Task<UserProfile> GetProfileAsync(CancellationToken ct)
        {
            if (_currentUser.ProfileId == Guid.Empty) throw new CurrentProfileUnavailableException();
            var profile = await _profiles.GetByIdAsync(_currentUser.ProfileId, ct);
            if (profile?.BarberShopId is null) throw new ForbiddenAccessException();
            return profile;
        }
    }
}
