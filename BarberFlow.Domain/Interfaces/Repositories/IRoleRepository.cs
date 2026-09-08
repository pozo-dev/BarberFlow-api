using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories;
public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int roleId, CancellationToken cancellationToken);
}
