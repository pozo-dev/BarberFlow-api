using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private readonly BarberFlowDbContext _context;

        public BranchRepository(BarberFlowDbContext context)
        {
            _context = context;
        }

        public async Task<List<Branch>> GetByBarberShopIdAsync(
            Guid barberShopId,
            CancellationToken cancellationToken)
        {
            return await _context.Branches
                .Include(x => x.LocationSearch)
                .Where(x => x.BarberShopId == barberShopId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Branch?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _context.Branches
                .Include(x => x.BarberShop)
                .Include(x => x.LocationSearch)
                .Include(x => x.Schedules)
                .Include(x => x.BarberAssignments)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<Branch?> GetMainBranchAsync(
            Guid barberShopId,
            CancellationToken cancellationToken)
        {
            return await _context.Branches
                .Include(x => x.Schedules)
                .Include(x => x.LocationSearch)
                .FirstOrDefaultAsync(
                    x => x.BarberShopId == barberShopId &&
                         x.IsMain,
                    cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(
            Guid barberShopId,
            string name,
            CancellationToken cancellationToken)
        {
            return await _context.Branches
                .AnyAsync(
                    x => x.BarberShopId == barberShopId &&
                         x.Name == name,
                    cancellationToken);
        }

        public async Task AddAsync(
            Branch branch,
            CancellationToken cancellationToken)
        {
            await _context.Branches.AddAsync(branch, cancellationToken);
        }

        public void Update(Branch branch)
        {
            _context.Branches.Update(branch);
        }

        public void Remove(Branch branch)
        {
            _context.Branches.Remove(branch);
        }
    }
}
