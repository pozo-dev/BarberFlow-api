using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly BarberFlowDbContext _context;

        public UserProfileRepository(BarberFlowDbContext context)
        {
            _context = context;
        }

        public void Add(UserProfile userProfile)
        {
            _context.UserProfiles.Add(userProfile);
        }

        public Task<UserProfile?> GetActiveByIdAndUserIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            return _context.UserProfiles
                    .FirstOrDefaultAsync(x =>
                        x.Id == id &&
                        x.IsActive &&
                        x.UserId == userId, 
                        cancellationToken);
        }

        public async Task<IReadOnlyCollection<UserProfile>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.UserProfiles
                .Where(x => x.UserId == userId && x.IsActive)
                .Include(x => x.Role)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid userId, int roleId, CancellationToken cancellationToken)
        {
            return await _context.UserProfiles
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.RoleId == roleId &&
                    x.IsActive,
                    cancellationToken);
        }
    }
}
