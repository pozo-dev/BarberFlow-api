using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Validation;
using BarberFlow.Application.Features.Users.Exceptions;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Users.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateUserResponseDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var normalizedPhone = PhoneNumberValidator.NormalizeAndValidate(request.PhoneNumber);

            Validate(normalizedPhone);

            var exists = await _userRepository.GetByPhoneNumberAsync(normalizedPhone, cancellationToken);

            if (exists != null)
                throw new UserAlreadyExistsException();

            var user = new User(normalizedPhone);

            _userRepository.Add(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateUserResponseDto
            {
                UserId = user.Id
            };
        }

        private static void Validate(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ValidationException("Invalid phone number.");
        }
    }
}
