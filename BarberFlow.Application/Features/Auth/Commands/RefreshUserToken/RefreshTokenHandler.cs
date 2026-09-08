using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Appointments.Exceptions;
using BarberFlow.Application.Features.Auth.DTOs;
using BarberFlow.Application.Features.Auth.Exceptions;
using BarberFlow.Domain.Constants;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using BarberFlow.Domain.Security;
using BarberFlow.Application.Common.Security;
using MediatR;

namespace BarberFlow.Application.Features.Auth.Commands.RefreshUserToken;
public class RefreshTokenHandler: IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserProfileRepository _userProfileRepository;

    public RefreshTokenHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtService jwtService,
        IUnitOfWork unitOfWork,
        IUserProfileRepository userProfileRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
        _userProfileRepository = userProfileRepository;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(
            Sha256Hasher.Hash(request.RefreshToken),
            cancellationToken);

        //if (token == null || !token.IsActive())
        if (token == null)
            throw new InvalidRefreshTokenException();

        if (token.IsRevoked)
        {
            if (token.RevocationReason == RevocationReasons.Replaced)
            {
                await _refreshTokenRepository
                    .RevokeAllByUserIdAsync(token.UserId, RevocationReasons.ReuseDetected, cancellationToken);

                throw new RefreshTokenReuseDetectedException();
            }

            throw new InvalidRefreshTokenException();
        }

        if (token.IsExpired())
            throw new InvalidRefreshTokenException();

        if (request.DeviceId != token.DeviceId)
            throw new InvalidRefreshTokenException();

        var user = await _userRepository.GetByIdAsync(token.UserId, cancellationToken);

        if (user == null)
            throw new UserNotFoundException();
        if (!user.IsActive)
            throw new UserNotActiveException();

        var userProfile = await _userProfileRepository.GetActiveByIdAndUserIdAsync(token.UserProfileId, user.Id, cancellationToken);
        if (userProfile == null)
            throw new UserProfileNotFoundException();

        var now = DateTimeOffset.UtcNow;

        // Crear nuevo token primero
        var newRefreshTokenValue = _jwtService.GenerateRefreshToken();

        // Revocar el anterior
        token.Revoke(RevocationReasons.Replaced);

        var newRefreshToken = RefreshToken.Create(
            user.Id,
            userProfile.Id,
            Sha256Hasher.Hash(newRefreshTokenValue),
            now.AddDays(AuthenticationSettings.RefreshTokenExpirationDays),
            token.DeviceId
        );

        _refreshTokenRepository.Add(newRefreshToken);

        // Nuevo access token
        var newAccessToken = _jwtService.GenerateAccessToken(user, userProfile);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            AccessTokenExpiration = _jwtService.GetAccessTokenExpiration()
        };
    }
}
