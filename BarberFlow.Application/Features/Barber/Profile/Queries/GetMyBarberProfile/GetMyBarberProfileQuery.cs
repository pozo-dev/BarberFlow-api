using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Constants;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Barber.Profile.Queries.GetMyBarberProfile;

public sealed record GetMyBarberProfileQuery : IRequest<BarberProfileDto>;

public sealed class GetMyBarberProfileHandler
    : IRequestHandler<GetMyBarberProfileQuery, BarberProfileDto>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserProfileRepository _profiles;
    private readonly ICollaboratorRepository _collaborators;

    public GetMyBarberProfileHandler(
        ICurrentUserService currentUser,
        IUserProfileRepository profiles,
        ICollaboratorRepository collaborators) =>
        (_currentUser, _profiles, _collaborators) =
        (currentUser, profiles, collaborators);

    public async Task<BarberProfileDto> Handle(
        GetMyBarberProfileQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.ProfileId == Guid.Empty)
            throw new CurrentProfileUnavailableException();

        var profile = await _profiles.GetByIdAsync(
            _currentUser.ProfileId,
            cancellationToken);
        if (profile?.RoleId != RoleIds.Barber || !profile.IsActive)
            throw new ForbiddenAccessException();

        var collaborators = await _collaborators.GetActiveByUserProfileIdAsync(
            profile.Id,
            cancellationToken);
        if (collaborators.Count == 0)
            throw new ForbiddenAccessException();

        var primary = collaborators[0];
        return new BarberProfileDto
        {
            FullName = primary.FullName,
            PhoneNumber = primary.PhoneNumber,
            Branches = collaborators.Select(collaborator => new BarberBranchDto
            {
                Id = collaborator.BranchId,
                CollaboratorId = collaborator.Id,
                Name = collaborator.Branch.Name,
                Address = collaborator.Branch.Address,
            }).ToList(),
        };
    }
}

public sealed class BarberProfileDto
{
    public string FullName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public IReadOnlyList<BarberBranchDto> Branches { get; init; } = [];
}

public sealed class BarberBranchDto
{
    public Guid CollaboratorId { get; init; }
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}
