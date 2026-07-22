using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories
{
    public interface IUserProfileRepository
    {
        void Add(UserProfile userProfile);

        Task<UserProfile?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<UserProfile?> GetActiveByIdAndUserIdAsync(
            Guid id, Guid userId,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<UserProfile>> GetActiveByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken);

        Task<bool> ExistsAsync(Guid userId,
            int roleId,
            CancellationToken cancellationToken);
    }
}
