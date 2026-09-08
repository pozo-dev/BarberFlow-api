using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Collaborators.Exceptions;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace BarberFlow.Application.Features.Collaborators.Commands.ToggleCollaboratorStatus;
public class ToggleCollaboratorStatusHandler : IRequestHandler<ToggleCollaboratorStatusCommand>
{
    private readonly ICurrentUserService _currentUser; private readonly IUserProfileRepository _profiles; private readonly ICollaboratorRepository _collaborators; private readonly IUnitOfWork _unitOfWork;
    public ToggleCollaboratorStatusHandler(ICurrentUserService currentUser, IUserProfileRepository profiles, ICollaboratorRepository collaborators, IUnitOfWork unitOfWork) => (_currentUser, _profiles, _collaborators, _unitOfWork) = (currentUser, profiles, collaborators, unitOfWork);
    public async Task Handle(ToggleCollaboratorStatusCommand request, CancellationToken ct)
    {
        if (_currentUser.ProfileId == Guid.Empty) throw new CurrentProfileUnavailableException(); var profile = await _profiles.GetByIdAsync(_currentUser.ProfileId, ct);
        if (profile?.BarberShopId is null) throw new ForbiddenAccessException(); var collaborator = await _collaborators.GetByIdAsync(request.Id, ct) ?? throw new CollaboratorNotFoundException();
        if (collaborator.Branch.BarberShopId != profile.BarberShopId) throw new ForbiddenAccessException(); if (collaborator.IsActive) collaborator.Deactivate(); else collaborator.Activate(); await _unitOfWork.SaveChangesAsync(ct);
    }
}
