using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories
{
    public interface IServiceRepository
    {
        Task<List<Service>> GetByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken);

        Task<List<Service>> GetByBarberShopIdAsync(
            Guid barberShopId,
            CancellationToken cancellationToken);

        Task<Service?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task AddAsync(
            Service service,
            CancellationToken cancellationToken);

        void Update(Service service);

        void Remove(Service service);
    }
}
