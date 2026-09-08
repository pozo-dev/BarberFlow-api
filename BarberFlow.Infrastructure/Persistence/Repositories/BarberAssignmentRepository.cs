using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories;
public class BarberAssignmentRepository : IBarberAssignmentRepository
{
    private readonly BarberFlowDbContext _context;

    public BarberAssignmentRepository(BarberFlowDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(
        Guid branchId,
        Guid barberProfileId,
        CancellationToken cancellationToken)
    {
        return await _context.BarberAssignments
            .AnyAsync(
                x => x.BranchId == branchId &&
                     x.BarberProfileId == barberProfileId,
                cancellationToken);
    }

    public async Task<List<BarberAssignment>> GetByBranchIdAsync(
        Guid branchId,
        CancellationToken cancellationToken)
    {
        return await _context.BarberAssignments
            .Where(x => x.BranchId == branchId && x.IsActive)
            .Include(x => x.BarberProfile)
                .ThenInclude(x => x.User)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.BarberProfile.User.PhoneNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<BarberAssignment?> GetAsync(
        Guid branchId,
        Guid barberProfileId,
        CancellationToken cancellationToken)
    {
        return await _context.BarberAssignments
            .FirstOrDefaultAsync(
                x => x.BranchId == branchId &&
                     x.BarberProfileId == barberProfileId,
                cancellationToken);
    }

    public async Task AddAsync(
        BarberAssignment assignment,
        CancellationToken cancellationToken)
    {
        await _context.BarberAssignments
            .AddAsync(assignment, cancellationToken);
    }

    public void Remove(BarberAssignment assignment)
    {
        _context.BarberAssignments.Remove(assignment);
    }
}
