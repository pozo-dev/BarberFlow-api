using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories;
public interface IBarberAssignmentRepository
{
    Task<bool> ExistsAsync(
        Guid branchId,
        Guid barberProfileId,
        CancellationToken cancellationToken);

    Task<List<BarberAssignment>> GetByBranchIdAsync(
        Guid branchId,
        CancellationToken cancellationToken);

    Task<BarberAssignment?> GetAsync(
        Guid branchId,
        Guid barberProfileId,
        CancellationToken cancellationToken);

    Task AddAsync(
        BarberAssignment assignment,
        CancellationToken cancellationToken);

    void Remove(BarberAssignment assignment);
}
