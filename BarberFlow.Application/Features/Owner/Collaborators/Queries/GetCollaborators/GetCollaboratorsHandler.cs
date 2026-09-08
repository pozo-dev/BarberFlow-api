using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace BarberFlow.Application.Features.Collaborators.Queries.GetCollaborators;
public class GetCollaboratorsHandler : IRequestHandler<GetCollaboratorsQuery, List<CollaboratorDto>>
{
    private readonly ICurrentUserService _currentUser; private readonly IUserProfileRepository _profiles; private readonly ICollaboratorRepository _collaborators;
    public GetCollaboratorsHandler(ICurrentUserService currentUser, IUserProfileRepository profiles, ICollaboratorRepository collaborators) => (_currentUser, _profiles, _collaborators) = (currentUser, profiles, collaborators);
    public async Task<List<CollaboratorDto>> Handle(GetCollaboratorsQuery request, CancellationToken ct)
    {
        if (_currentUser.ProfileId == Guid.Empty) throw new CurrentProfileUnavailableException();
        var profile = await _profiles.GetByIdAsync(_currentUser.ProfileId, ct);
        if (profile?.BarberShopId is null) throw new ForbiddenAccessException();
        var collaborators = await _collaborators.GetByBarberShopIdAsync(profile.BarberShopId.Value, ct);
        return collaborators.Select(x => new CollaboratorDto { Id = x.Id, BranchId = x.BranchId, FullName = x.FullName, PhoneNumber = x.PhoneNumber, IsActive = x.IsActive, CreatedAt = x.CreatedAt, UpdatedAt = x.UpdatedAt }).ToList();
    }
}
