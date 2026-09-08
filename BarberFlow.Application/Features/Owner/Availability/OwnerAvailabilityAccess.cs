using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Constants;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces.Repositories;

namespace BarberFlow.Application.Features.Owner.Availability;

public sealed class OwnerAvailabilityAccess(
    ICurrentUserService currentUser,
    IUserProfileRepository profiles,
    ICollaboratorRepository collaborators,
    IBranchRepository branches)
{
    public async Task<Guid> GetShopIdAsync(CancellationToken ct)
    {
        var profile = await profiles.GetByIdAsync(currentUser.ProfileId, ct);
        if (profile is null ||
            !profile.IsActive ||
            profile.RoleId != RoleIds.Owner ||
            !profile.BarberShopId.HasValue)
        {
            throw new ForbiddenAccessException();
        }

        return profile.BarberShopId.Value;
    }

    public async Task<Branch> ResolveBranchAsync(Guid collaboratorId, CancellationToken ct)
    {
        var shopId = await GetShopIdAsync(ct);
        var collaborator = await collaborators.GetByIdAsync(collaboratorId, ct);
        if (collaborator is null || collaborator.Branch.BarberShopId != shopId)
        {
            throw new ForbiddenAccessException();
        }

        return await branches.GetByIdAsync(collaborator.BranchId, ct)
            ?? throw new ForbiddenAccessException();
    }
}
