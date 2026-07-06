using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Common.Security;
using BarberFlow.Application.Common.Utils;
using BarberFlow.Application.Common.Validation;
using BarberFlow.Application.Features.Auth.DTOs;
using BarberFlow.Application.Features.Auth.Exceptions;
using BarberFlow.Domain.Constants;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using BarberFlow.Domain.Security;
using MediatR;
using System.Text.RegularExpressions;

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
        //    try
        //    {
        //        var normalizedPhone = PhoneNumberUtils.NormalizePhoneNumber(request.PhoneNumber);
        //        if (string.IsNullOrWhiteSpace(normalizedPhone) || !Regex.IsMatch(normalizedPhone, @"^\d{10,15}$"))
        //            throw new InvalidCredentialsException();

        //        if (string.IsNullOrWhiteSpace(request.Code) || !Regex.IsMatch(request.Code, @"^\d{4,8}$"))
        //            throw new InvalidCredentialsException();

        //        if (string.IsNullOrWhiteSpace(request.DeviceId))
        //            throw new MissingDeviceIdException();

        //        var user = await _userRepository.GetByPhoneNumberAsync(normalizedPhone, cancellationToken);

        //        if (user == null)
        //            throw new UserNotFoundException();

        //        if (!user.IsActive)
        //            throw new UserNotActiveException();

        //        const int MaxOtpAttempts = 5;
        //        var otp = await _otpCodeRepository.GetLastValidOtpAsync(user.Id, cancellationToken);

        //        if (otp == null || !otp.IsValid(request.Code, MaxOtpAttempts) || otp.IsBlocked(MaxOtpAttempts))
        //        {
        //            if (otp != null)
        //            {
        //                otp.IncrementFailedAttempts();
        //                await _unitOfWork.SaveChangesAsync(cancellationToken);
        //            }
        //            throw new InvalidCredentialsException();
        //        }

        //        if (!otp.RequestedUserProfileId.HasValue)
        //            throw new UserProfileNotFoundException();

        //        var userProfile = await _userProfileRepository
        //            .GetActiveByIdAndUserIdAsync(otp.RequestedUserProfileId.Value, cancellationToken);

        //        if (userProfile == null)
        //            throw new UserProfileNotFoundException();

        //        // Revocar todos los refresh tokens activos para este usuario y dispositivo
        //        var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);
        //        foreach (var token in activeTokens.Where(t => t.DeviceId == request.DeviceId && t.IsActive()))
        //        {
        //            token.Revoke();
        //        }

        //        // Generar tokens nuevos
        //        var accessToken = _jwtService.GenerateAccessToken(user, userProfile);
        //        var refreshTokenValue = _jwtService.GenerateRefreshToken();
        //        var refreshExpiration = DateTime.UtcNow.AddDays(7);

        //        var encryptedRefreshToken = _tokenEncryptionService.Encrypt(refreshTokenValue);
        //        var refreshToken = RefreshToken.Create(user.Id, userProfile.Id, encryptedRefreshToken, refreshExpiration, request.DeviceId);

        //        _refreshTokenRepository.Add(refreshToken);

        //        otp.MarkAsUsed();

        //        await _unitOfWork.SaveChangesAsync(cancellationToken);

        //        return new AuthResponseDto
        //        {
        //            AccessToken = accessToken,
        //            RefreshToken = refreshTokenValue,
        //            AccessTokenExpiration = _jwtService.GetAccessTokenExpiration()
        //        };
        //    }
        //    catch (UserNotFoundException)
        //    {
        //        throw new InvalidCredentialsException();
        //    }
        //    catch (UserNotActiveException)
        //    {
        //        throw new InvalidCredentialsException();
        //    }
        //    catch (InvalidOtpException)
        //    {
        //        throw new InvalidCredentialsException();
        //    }
        //}

        public async Task<AuthResponseDto> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var normalizedPhone = ValidateRequest(request);

            var user = await GetUserAsync(normalizedPhone, cancellationToken);

            var codeHash = Sha256Hasher.Hash(request.Code);
            var otp = await GetValidOtpAsync(user, codeHash, cancellationToken);

            var userProfile = await GetUserProfileAsync(otp, cancellationToken);

            await RevokeRefreshTokensAsync(user.Id, request.DeviceId, cancellationToken);

            var refreshToken = CreateRefreshToken(user, userProfile, request.DeviceId);

            _refreshTokenRepository.Add(refreshToken);

            otp.MarkAsUsed();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return CreateAuthResponse(user, userProfile, refreshToken.Token);
        }

        private static string ValidateRequest(VerifyOtpCommand request)
        {
            var normalizedPhone = PhoneNumberValidator.NormalizeAndValidate(request.PhoneNumber);

            if (string.IsNullOrWhiteSpace(request.Code) ||
                !Regex.IsMatch(request.Code, @"^\d{4,8}$"))
            {
                throw new ValidationException("Invalid OTP format");
            }

            if (string.IsNullOrWhiteSpace(request.DeviceId))
            {
                throw new ValidationException("DeviceId is required");
            }

            return normalizedPhone;
        }

        private async Task<User> GetUserAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber, cancellationToken);

            if (user == null || !user.IsActive)
                throw new InvalidCredentialsException();

            return user;
        }

        private async Task<OtpCode> GetValidOtpAsync(User user, string codeHash, CancellationToken cancellationToken)
        {
            var otp = await _otpCodeRepository.GetLastValidOtpAsync(user.Id, cancellationToken);

            if (otp == null)
                throw new InvalidCredentialsException();

            if (otp.IsBlocked(AuthenticationSettings.OtpMaxAttempts))
                throw new InvalidCredentialsException();

            if (!otp.IsValid(codeHash, AuthenticationSettings.OtpMaxAttempts))
            {
                otp.IncrementFailedAttempts();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new InvalidCredentialsException();
            }

            return otp;
        }

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

        //private AuthResponseDto CreateSession(User user, UserProfile userProfile, string deviceId)
        //{
        //    var refreshTokenValue = _jwtService.GenerateRefreshToken();

        //    var encryptedRefreshToken =
        //        _tokenEncryptionService.Encrypt(refreshTokenValue);

        //    var refreshToken = RefreshToken.Create(
        //        user.Id,
        //        userProfile.Id,
        //        encryptedRefreshToken,
        //        DateTime.UtcNow.AddDays(7),
        //        deviceId);

        //    _refreshTokenRepository.Add(refreshToken);

        //    return new AuthResponseDto
        //    {
        //        AccessToken = _jwtService.GenerateAccessToken(user, userProfile),
        //        RefreshToken = refreshTokenValue,
        //        AccessTokenExpiration = _jwtService.GetAccessTokenExpiration()
        //    };
        //}

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
    }
}
