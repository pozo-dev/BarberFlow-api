using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Common.Validation;
using BarberFlow.Application.Features.Branches.Exceptions;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Collaborators.Commands.CreateCollaborator
{
    public class CreateCollaboratorHandler : IRequestHandler<CreateCollaboratorCommand, Guid>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IUserProfileRepository _profiles;
        private readonly IBranchRepository _branches;
        private readonly ICollaboratorRepository _collaborators;
        private readonly IUnitOfWork _unitOfWork;
        
        public CreateCollaboratorHandler(ICurrentUserService currentUser,
            IUserProfileRepository profiles,
            IBranchRepository branches,
            ICollaboratorRepository collaborators,
            IUnitOfWork unitOfWork)
            => (_currentUser, _profiles, _branches, _collaborators, _unitOfWork) =
                (currentUser, profiles, branches, collaborators, unitOfWork);
        
        public async Task<Guid> Handle(CreateCollaboratorCommand request, CancellationToken cancellationToken)
        {
            var profile = await GetCurrentProfileAsync(cancellationToken);
            var branch = await _branches.GetByIdAsync(request.BranchId, cancellationToken) ?? throw new BranchNotFoundException();
            if (branch.BarberShopId != profile.BarberShopId) throw new ForbiddenAccessException();
            Collaborator collaborator;
            try { collaborator = Collaborator.Create(request.BranchId, request.FullName, PhoneNumberValidator.NormalizeAndValidate(request.PhoneNumber)); }
            catch (ArgumentException exception) { throw new ValidationException(exception.Message); }
            await _collaborators.AddAsync(collaborator, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return collaborator.Id;
        }

        private async Task<UserProfile> GetCurrentProfileAsync(CancellationToken ct)
        {
            if (_currentUser.ProfileId == Guid.Empty) throw new CurrentProfileUnavailableException();
            var profile = await _profiles.GetByIdAsync(_currentUser.ProfileId, ct);
            if (profile?.BarberShopId is null) throw new ForbiddenAccessException();
            return profile;
        }
    }
}
