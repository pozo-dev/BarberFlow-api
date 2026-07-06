using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories
{
    public interface IServiceRepository
    {
        Task<List<Service>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);
    }
}
