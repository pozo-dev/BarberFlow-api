using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly BarberFlowDbContext _context;

        public RefreshTokenRepository(BarberFlowDbContext context)
        {
            _context = context;
        }

        public void Add(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == token && x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow, cancellationToken);
        }

        public async Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .Where(x => x.UserId == userId && x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }

        public async Task RevokeAllByUserIdAsync(Guid userId, string reason, CancellationToken cancellationToken)
        {
            var tokens = await _context.RefreshTokens
                .Where(x => x.UserId == userId && x.RevokedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var token in tokens)
            {
                token.Revoke(reason);
            }
        }
    }
}
