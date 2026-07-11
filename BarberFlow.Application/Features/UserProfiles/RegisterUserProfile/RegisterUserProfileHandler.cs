using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Features.Appointments.Exceptions;
using BarberFlow.Application.Features.Roles.Exceptions;
using BarberFlow.Domain.Constants;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.UserProfiles.RegisterUserProfile
{
    public class RegisterUserProfileHandler
    : IRequestHandler<RegisterUserProfileCommand, RegisterUserProfileResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserProfileHandler(
            IUserRepository userRepository,
            IUserProfileRepository userProfileRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _roleRepository = roleRepository;
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

            if (request.RoleId != RoleIds.Client &&
                request.RoleId != RoleIds.Barber)
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
}
