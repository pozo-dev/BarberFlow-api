using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories
{
    public class BarberShopRepository : IBarberShopRepository
    {
        private readonly BarberFlowDbContext _context;

        public BarberShopRepository(BarberFlowDbContext context)
        {
            _context = context;
        }

        public Task<BarberShop?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _context.BarberShops
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }
    }
}