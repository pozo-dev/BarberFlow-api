using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories
{
    public interface IBranchRepository
    {
        Task<List<Branch>> GetByBarberShopIdAsync(
            Guid barberShopId,
            CancellationToken cancellationToken);

        Task<Branch?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<Branch?> GetMainBranchAsync(
            Guid barberShopId,
            CancellationToken cancellationToken);

        Task<bool> ExistsByNameAsync(
            Guid barberShopId,
            string name,
            CancellationToken cancellationToken);

        Task AddAsync(
            Branch branch,
            CancellationToken cancellationToken);

        void Update(Branch branch);

        void Remove(Branch branch);
    }
}