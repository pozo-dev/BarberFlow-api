using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Collaborators.Exceptions;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;
namespace BarberFlow.Application.Features.Collaborators.Queries.GetCollaboratorById
{
    public class GetCollaboratorByIdHandler : IRequestHandler<GetCollaboratorByIdQuery, CollaboratorDto>
    {
        private readonly ICurrentUserService _currentUser; private readonly IUserProfileRepository _profiles; private readonly ICollaboratorRepository _collaborators;
        public GetCollaboratorByIdHandler(ICurrentUserService currentUser, IUserProfileRepository profiles, ICollaboratorRepository collaborators) => (_currentUser, _profiles, _collaborators) = (currentUser, profiles, collaborators);
        public async Task<CollaboratorDto> Handle(GetCollaboratorByIdQuery request, CancellationToken ct)
        {
            if (_currentUser.ProfileId == Guid.Empty) throw new CurrentProfileUnavailableException();
            var profile = await _profiles.GetByIdAsync(_currentUser.ProfileId, ct);
            if (profile?.BarberShopId is null) throw new ForbiddenAccessException();
            var x = await _collaborators.GetByIdAsync(request.Id, ct) ?? throw new CollaboratorNotFoundException();
            if (x.Branch.BarberShopId != profile.BarberShopId) throw new ForbiddenAccessException();
            return new CollaboratorDto { Id = x.Id, BranchId = x.BranchId, FullName = x.FullName, PhoneNumber = x.PhoneNumber, IsActive = x.IsActive, CreatedAt = x.CreatedAt, UpdatedAt = x.UpdatedAt };
        }
    }
}
