using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories;

public interface ILocationSearchRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<LocationSearch>> SearchAsync(string searchText, int take, CancellationToken cancellationToken);
}
