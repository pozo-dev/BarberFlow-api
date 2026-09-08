using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories;
public interface ICollaboratorRepository
{
    Task<List<Collaborator>> GetByBarberShopIdAsync(Guid barberShopId, CancellationToken cancellationToken);
    Task<List<Collaborator>> GetActiveByBranchIdAsync(Guid branchId, CancellationToken cancellationToken);
    Task<List<Collaborator>> GetActiveByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
    Task<List<Collaborator>> GetActiveByUserProfileIdAsync(Guid userProfileId, CancellationToken cancellationToken);
    Task<Collaborator?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Collaborator collaborator, CancellationToken cancellationToken);
}
