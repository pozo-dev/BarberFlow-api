using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Auth.DTOs;
using BarberFlow.Application.Features.Auth.Exceptions;
using BarberFlow.Domain.Constants;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using BarberFlow.Domain.Security;
using MediatR;

namespace BarberFlow.Application.Features.Auth.Commands.VerifyOtp
{
    public class VerifyOtpHandler : IRequestHandler<VerifyOtpCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IOtpCodeRepository _otpCodeRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenEncryptionService _tokenEncryptionService;

        public VerifyOtpHandler(
            IUserRepository userRepository,
            IUserProfileRepository userProfileRepository,
            IOtpCodeRepository otpRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtService jwtService,
            IUnitOfWork unitOfWork,
            ITokenEncryptionService tokenEncryptionService)
        {
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _otpCodeRepository = otpRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
            _tokenEncryptionService = tokenEncryptionService;
        }

        //public async Task<AuthResponseDto> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        //{
        //    var normalizedPhone = ValidateRequest(request);

        //    var user = await GetUserAsync(normalizedPhone, cancellationToken);

        //    var codeHash = Sha256Hasher.Hash(request.Code);
        //    var otp = await GetValidOtpAsync(user, codeHash, cancellationToken);

        //    var userProfile = await GetUserProfileAsync(otp, cancellationToken);

        //    await RevokeRefreshTokensAsync(user.Id, request.DeviceId, cancellationToken);

        //    var refreshToken = CreateRefreshToken(user, userProfile, request.DeviceId);

        //    _refreshTokenRepository.Add(refreshToken);

        //    otp.MarkAsUsed();

        //    await _unitOfWork.SaveChangesAsync(cancellationToken);

        //    return CreateAuthResponse(user, userProfile, refreshToken.Token);
        //}

        public async Task<AuthResponseDto> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            ValidateRequest(request);

            var otp = await GetValidOtpAsync(
                request.OtpId,
                cancellationToken);

            var user = await GetUserAsync(
                otp.UserId,
                cancellationToken);

            var userProfile = await GetUserProfileAsync(
                otp,
                cancellationToken);

            await RevokeRefreshTokensAsync(
                user.Id,
                request.DeviceId,
                cancellationToken);

            var refreshToken = CreateRefreshToken(
                user,
                userProfile,
                request.DeviceId);

            _refreshTokenRepository.Add(refreshToken);

            otp.MarkAsUsed();

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return CreateAuthResponse(
                user,
                userProfile,
                refreshToken.Token);
        }

        //private static string ValidateRequest(VerifyOtpCommand request)
        //{
        //    var normalizedPhone = PhoneNumberValidator.NormalizeAndValidate(request.PhoneNumber);

        //    if (string.IsNullOrWhiteSpace(request.Code) ||
        //        !Regex.IsMatch(request.Code, @"^\d{4,8}$"))
        //    {
        //        throw new ValidationException("Invalid OTP format");
        //    }

        //    if (string.IsNullOrWhiteSpace(request.DeviceId))
        //    {
        //        throw new ValidationException("DeviceId is required");
        //    }

        //    return normalizedPhone;
        //}

        private static void ValidateRequest(VerifyOtpCommand request)
        {
            if (request.OtpId == Guid.Empty)
                throw new ValidationException("OtpId is required.");

            if (string.IsNullOrWhiteSpace(request.DeviceId))
                throw new ValidationException("DeviceId is required.");
        }

        //private async Task<User> GetUserAsync(string phoneNumber, CancellationToken cancellationToken)
        //{
        //    var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber, cancellationToken);

        //    if (user == null || !user.IsActive)
        //        throw new InvalidCredentialsException();

        //    return user;
        //}

    //    private async Task<OtpCode> GetValidOtpAsync(User user, string codeHash, CancellationToken cancellationToken)
    //    {
    //        //var otp = await _otpCodeRepository.GetLastValidOtpAsync(user.Id, cancellationToken);
    //        var otp = await _otpCodeRepository.GetByIdAsync(
    //request.OtpId,
    //cancellationToken);
    //        if (otp == null)
    //            throw new InvalidCredentialsException();

    //        if (otp.IsBlocked(AuthenticationSettings.OtpMaxAttempts))
    //            throw new InvalidCredentialsException();

    //        if (!otp.IsValid(codeHash, AuthenticationSettings.OtpMaxAttempts))
    //        {
    //            otp.IncrementFailedAttempts();

    //            await _unitOfWork.SaveChangesAsync(cancellationToken);

    //            throw new InvalidCredentialsException();
    //        }

    //        return otp;
    //    }

        private async Task<UserProfile> GetUserProfileAsync(OtpCode otp, CancellationToken cancellationToken)
        {
            if (!otp.RequestedUserProfileId.HasValue)
                throw new InvalidCredentialsException();

            var profile = await _userProfileRepository
                .GetActiveByIdAndUserIdAsync(otp.RequestedUserProfileId.Value, otp.UserId, cancellationToken);

            if (profile == null)
                throw new InvalidCredentialsException();

            return profile;
        }

        private async Task RevokeRefreshTokensAsync(Guid userId, string deviceId, CancellationToken cancellationToken)
        {
            var activeTokens = await _refreshTokenRepository
                .GetActiveByUserIdAsync(userId, cancellationToken);

            foreach (var token in activeTokens.Where(x =>
                         x.DeviceId == deviceId &&
                         x.IsActive()))
            {
                token.Revoke(RevocationReasons.Replaced);
            }
        }

        private RefreshToken CreateRefreshToken(User user, UserProfile userProfile, string deviceId)
        {
            var refreshTokenValue = _jwtService.GenerateRefreshToken();

            var encryptedRefreshToken = _tokenEncryptionService.Encrypt(refreshTokenValue);

            return RefreshToken.Create(
                user.Id,
                userProfile.Id,
                encryptedRefreshToken,
                DateTime.UtcNow.AddDays(AuthenticationSettings.RefreshTokenExpirationDays),
                deviceId);
        }

        private AuthResponseDto CreateAuthResponse(User user, UserProfile userProfile, string refreshToken)
        {
            var accessToken = _jwtService.GenerateAccessToken(user, userProfile);
            var accessTokenExpiration = _jwtService.GetAccessTokenExpiration();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiration = accessTokenExpiration
            };

            //return new AuthResponseDto
            //{
            //    AccessToken = _jwtService.GenerateAccessToken(user, userProfile),
            //    RefreshToken = refreshToken,
            //    AccessTokenExpiration = _jwtService.GetAccessTokenExpiration()
            //};
        }

        private async Task<User> GetUserAsync(
    Guid userId,
    CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(
                userId,
                cancellationToken);

            if (user == null || !user.IsActive)
                throw new InvalidCredentialsException();

            return user;
        }

        private async Task<OtpCode> GetValidOtpAsync(
    Guid otpId,
    CancellationToken cancellationToken)
        {
            var otp = await _otpCodeRepository.GetByIdAsync(
                otpId,
                cancellationToken);

            if (otp == null)
                throw new InvalidCredentialsException();

            //if (!otp.IsActive)
            //    throw new InvalidCredentialsException();

            if (otp.IsUsed)
                throw new InvalidCredentialsException();

            //if (otp.IsExpired())
            //    throw new InvalidCredentialsException();

            if (otp.IsBlocked(AuthenticationSettings.OtpMaxAttempts))
                throw new InvalidCredentialsException();

            return otp;
        }
    }
}
