using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace BarberFlow.Application.Features.Collaborators.Queries.GetCollaborators;
public class GetCollaboratorsHandler : IRequestHandler<GetCollaboratorsQuery, List<CollaboratorDto>>
{
    private readonly ICurrentUserService _currentUser; private readonly IUserProfileRepository _profiles; private readonly ICollaboratorRepository _collaborators; private readonly IAvailabilityRequestRepository _availability;
    public GetCollaboratorsHandler(ICurrentUserService currentUser, IUserProfileRepository profiles, ICollaboratorRepository collaborators, IAvailabilityRequestRepository availability) => (_currentUser, _profiles, _collaborators, _availability) = (currentUser, profiles, collaborators, availability);
    public async Task<List<CollaboratorDto>> Handle(GetCollaboratorsQuery request, CancellationToken ct)
    {
        if (_currentUser.ProfileId == Guid.Empty) throw new CurrentProfileUnavailableException();
        var profile = await _profiles.GetByIdAsync(_currentUser.ProfileId, ct);
        if (profile?.BarberShopId is null) throw new ForbiddenAccessException();
        var collaborators = await _collaborators.GetByBarberShopIdAsync(profile.BarberShopId.Value, ct);
        var pendingByCollaborator = await _availability
            .GetPendingCountsByCollaboratorAsync(profile.BarberShopId.Value, ct);
        return collaborators.Select(x => new CollaboratorDto { Id = x.Id, BranchId = x.BranchId, FullName = x.FullName, PhoneNumber = x.PhoneNumber, IsActive = x.IsActive, PendingAvailabilityRequests = pendingByCollaborator.GetValueOrDefault(x.Id), CreatedAt = x.CreatedAt, UpdatedAt = x.UpdatedAt }).ToList();
    }
}
