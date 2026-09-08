using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Features.Appointments.Exceptions;
using BarberFlow.Application.Features.Roles.Exceptions;
using BarberFlow.Domain.Constants;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.UserProfiles.RegisterUserProfile;
public class RegisterUserProfileHandler
: IRequestHandler<RegisterUserProfileCommand, RegisterUserProfileResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICollaboratorRepository _collaboratorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserProfileHandler(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        IRoleRepository roleRepository,
        ICollaboratorRepository collaboratorRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _roleRepository = roleRepository;
        _collaboratorRepository = collaboratorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterUserProfileResponseDto> Handle(
        RegisterUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        var user = await ValidateRequest(
            request,
            cancellationToken);

        var profile = new UserProfile(
            user.Id,
            request.RoleId,
            request.BarberShopId);

        _userProfileRepository.Add(profile);

        if (request.RoleId == RoleIds.Barber)
        {
            var collaborators = await _collaboratorRepository.GetActiveByPhoneNumberAsync(user.PhoneNumber, cancellationToken);
            if (collaborators.Count == 0)
                throw new ValidationException("Solo un colaborador activo puede crear un perfil de Barbero.");

            foreach (var collaborator in collaborators)
                collaborator.LinkUserProfile(profile.Id);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterUserProfileResponseDto
        {
            UserProfileId = profile.Id
        };
    }

    private async Task<User> ValidateRequest(
        RegisterUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
            throw new ValidationException("UserId is required.");

        if (request.BarberShopId.HasValue)
            throw new ValidationException("A barber shop cannot be assigned when registering a profile.");

        if (request.RoleId != RoleIds.Client &&
            request.RoleId != RoleIds.Barber &&
            request.RoleId != RoleIds.Owner)
        {
            throw new ValidationException(
                "The selected role cannot be self-registered.");
        }

        var user = await _userRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user == null)
            throw new UserNotFoundException();

        if (!user.IsActive)
            throw new UserNotActiveException();

        var role = await _roleRepository.GetByIdAsync(
            request.RoleId,
            cancellationToken);

        if (role == null)
            throw new RoleNotFoundException();

        var exists = await _userProfileRepository.ExistsAsync(
            request.UserId,
            request.RoleId,
            cancellationToken);

        if (exists)
            throw new UserProfileAlreadyExistsException();

        return user;
    }
}
