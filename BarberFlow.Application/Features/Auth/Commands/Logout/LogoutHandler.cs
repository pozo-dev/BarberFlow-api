using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Security;
using BarberFlow.Application.Features.Auth.Exceptions;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using BarberFlow.Domain.Security;
using MediatR;

namespace BarberFlow.Application.Features.Auth.Commands.Logout
{
    public sealed class LogoutHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LogoutHandler(
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            ValidateRequest(request);

            var token = await _refreshTokenRepository
                .GetByTokenAsync(
                    Sha256Hasher.Hash(request.RefreshToken),
                    cancellationToken);

            if (token == null)
                throw new InvalidRefreshTokenException();

            if (token.IsRevoked)
                throw new InvalidRefreshTokenException();

            if (token.IsExpired())
                throw new InvalidRefreshTokenException();

            if (!string.Equals(token.DeviceId, request.DeviceId, StringComparison.Ordinal))
                throw new InvalidRefreshTokenException();

            token.Revoke(RevocationReasons.Logout);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static void ValidateRequest(LogoutCommand request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                throw new ValidationException("Refresh token is required.");

            if (string.IsNullOrWhiteSpace(request.DeviceId))
                throw new ValidationException("Device ID is required.");
        }
    }
}
