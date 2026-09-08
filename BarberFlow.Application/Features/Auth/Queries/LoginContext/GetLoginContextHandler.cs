using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Validation;
using BarberFlow.Application.Features.Auth.DTOs;
using BarberFlow.Domain.Constants;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Auth.Queries.LoginContext;
public class GetLoginContextHandler : IRequestHandler<GetLoginContextQuery, LoginContextResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ICollaboratorRepository _collaboratorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetLoginContextHandler(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        ICollaboratorRepository collaboratorRepository,
        IUnitOfWork UnitOfWork)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _collaboratorRepository = collaboratorRepository;
        _unitOfWork = UnitOfWork;
    }

    public async Task<LoginContextResponseDto> Handle(GetLoginContextQuery request, CancellationToken cancellationToken)
    {
        var normalizedPhone = ValidateRequest(request);

        var user = await _userRepository.GetByPhoneNumberAsync(normalizedPhone, cancellationToken);

        if (user == null)
        {
            var collaboratorRoles = await GetEligibleRolesAsync(normalizedPhone, [], cancellationToken);
            return new LoginContextResponseDto
            {
                IsNewUser = true,
                UserId = Guid.Empty,
                Profiles = new List<LoginContextProfileDto>(),
                EligibleNewProfileRoleIds = collaboratorRoles
            };
        }

        var profiles = await _userProfileRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);

        var profileDtos = profiles
            .Select(x => new LoginContextProfileDto
            {
                UserProfileId = x.Id,
                RoleId = x.RoleId,
                RoleName = x.Role.Name
            })
            .ToList();

        return new LoginContextResponseDto
        {
            IsNewUser = false,
            UserId = user.Id,
            Profiles = profileDtos,
            EligibleNewProfileRoleIds = await GetEligibleRolesAsync(normalizedPhone, profileDtos.Select(x => x.RoleId), cancellationToken)
        };
    }

    private async Task<IReadOnlyCollection<int>> GetEligibleRolesAsync(string phoneNumber, IEnumerable<int> existingRoleIds, CancellationToken cancellationToken)
    {
        var existing = existingRoleIds.ToHashSet();
        var eligible = new List<int>();
        if (!existing.Contains(RoleIds.Client)) eligible.Add(RoleIds.Client);
        if (!existing.Contains(RoleIds.Owner)) eligible.Add(RoleIds.Owner);
        if (!existing.Contains(RoleIds.Barber) && (await _collaboratorRepository.GetActiveByPhoneNumberAsync(phoneNumber, cancellationToken)).Count > 0)
            eligible.Add(RoleIds.Barber);
        return eligible;
    }

    private static string ValidateRequest(GetLoginContextQuery request)
    {
        var normalizedPhone = PhoneNumberValidator.NormalizeAndValidate(request.PhoneNumber);

        if (string.IsNullOrWhiteSpace(normalizedPhone))
            throw new ValidationException("Invalid phone number.");

        return normalizedPhone;
    }
}
