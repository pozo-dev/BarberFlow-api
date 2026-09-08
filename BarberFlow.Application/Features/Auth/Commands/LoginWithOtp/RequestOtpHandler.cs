using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Security;
using BarberFlow.Application.Features.Appointments.Exceptions;
using BarberFlow.Application.Features.Auth.DTOs;
using BarberFlow.Domain.Constants;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using BarberFlow.Domain.Security;
using MediatR;

namespace BarberFlow.Application.Features.Auth.Commands.LoginWithOtp;
public class RequestOtpHandler: IRequestHandler<RequestOtpCommand, OtpResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IOtpCodeRepository _otpCodeRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RequestOtpHandler(IUserRepository userRepository,
        IOtpCodeRepository otpCodeRepository,
        IUserProfileRepository userProfileRepository,
        IUnitOfWork UnitOfWork)
    {
        _userRepository = userRepository;
        _otpCodeRepository = otpCodeRepository;
        _userProfileRepository = userProfileRepository;
        _unitOfWork = UnitOfWork;
    }

    public async Task<OtpResponseDto> Handle(RequestOtpCommand request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new UserNotFoundException();

        if (!user.IsActive)
            throw new UserNotActiveException();

        var profile = await _userProfileRepository.GetActiveByIdAndUserIdAsync(
            request.UserProfileId, user.Id, cancellationToken);

        if (profile == null)
            throw new UserProfileNotFoundException();

        await _otpCodeRepository.InvalidateAllAsync(user.Id, cancellationToken);

        var response = CreateOtp(user.Id, request.UserProfileId);
        _otpCodeRepository.Add(response.Otp);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return response.Dto;
    }

    private static (OtpCode Otp, OtpResponseDto Dto) CreateOtp(Guid userId, Guid userProfileId)
    {
        var code = OtpGenerator.Generate();

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(AuthenticationSettings.OtpExpirationMinutes);

        var otp = new OtpCode(
            userId,
            userProfileId,
            Sha256Hasher.Hash(code),
            expiresAt);

        var dto = new OtpResponseDto
        {
            //OtpCode = code,
            OtpId = otp.Id,
            ExpiresAt = expiresAt
        };

        return (otp, dto);
    }

    private static void ValidateRequest(RequestOtpCommand request)
    {
        if (request.UserId == Guid.Empty)
            throw new ValidationException("User is required.");

        if (request.UserProfileId == Guid.Empty)
            throw new ValidationException("User profile is required.");
    }
}
