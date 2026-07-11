using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Utils;
using BarberFlow.Application.Common.Validation;
using BarberFlow.Application.Features.Auth.DTOs;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Auth.Queries.LoginContext
{
    public class GetLoginContextHandler : IRequestHandler<GetLoginContextQuery, LoginContextResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GetLoginContextHandler(
            IUserRepository userRepository,
            IUserProfileRepository userProfileRepository,
            IUnitOfWork UnitOfWork)
        {
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _unitOfWork = UnitOfWork;
        }

        public async Task<LoginContextResponseDto> Handle(GetLoginContextQuery request, CancellationToken cancellationToken)
        {
            var normalizedPhone = ValidateRequest(request);

            var user = await _userRepository.GetByPhoneNumberAsync(normalizedPhone, cancellationToken);

            if (user == null)
            {
                return new LoginContextResponseDto
                {
                    IsNewUser = true,
                    UserId = Guid.Empty,
                    Profiles = new List<LoginContextProfileDto>()
                };
            }

            var profiles = await _userProfileRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);

            return new LoginContextResponseDto
            {
                IsNewUser = false,
                UserId = user.Id,
                Profiles = profiles
                    .Select(x => new LoginContextProfileDto
                    {
                        UserProfileId = x.Id,
                        RoleId = x.RoleId,
                        RoleName = x.Role.Name
                    })
                    .ToList()
            };
        }

        private static string ValidateRequest(GetLoginContextQuery request)
        {
            var normalizedPhone = PhoneNumberValidator.NormalizeAndValidate(request.PhoneNumber);

            if (string.IsNullOrWhiteSpace(normalizedPhone))
                throw new ValidationException("Invalid phone number.");

            return normalizedPhone;
        }
    }
}
