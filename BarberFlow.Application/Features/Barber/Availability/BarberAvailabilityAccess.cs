using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Constants;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces.Repositories;

namespace BarberFlow.Application.Features.Barber.Availability;

public sealed class BarberAvailabilityAccess(ICurrentUserService currentUser, IUserProfileRepository profiles,
    ICollaboratorRepository collaborators, IBranchRepository branches)
{
    public async Task<(Collaborator Collaborator, Branch Branch)> ResolveAsync(Guid collaboratorId, CancellationToken ct)
    {
        var profile = await profiles.GetByIdAsync(currentUser.ProfileId, ct);
        if (profile is null || !profile.IsActive || profile.RoleId != RoleIds.Barber)
            throw new ForbiddenAccessException();
        var collaborator = await collaborators.GetByIdAsync(collaboratorId, ct);
        if (collaborator is null || !collaborator.IsActive || collaborator.UserProfileId != profile.Id)
            throw new ForbiddenAccessException();
        var branch = await branches.GetByIdAsync(collaborator.BranchId, ct);
        if (branch is null || !branch.IsActive || !branch.BarberShop.IsActive)
            throw new ForbiddenAccessException();
        return (collaborator, branch);
    }
}
