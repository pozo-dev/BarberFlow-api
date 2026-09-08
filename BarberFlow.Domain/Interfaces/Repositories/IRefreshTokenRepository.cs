using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories;
public interface IRefreshTokenRepository
{
    void Add(RefreshToken token);

    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);

    Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task RevokeAllByUserIdAsync(Guid userId, string reason, CancellationToken cancellationToken);
}
