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

        public Task<BarberShop?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return _context.BarberShops
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    b => b.Id == id,
                    cancellationToken);
        }

        public async Task<BarberShop?> GetByOwnerUserIdAsync(
            Guid ownerUserId,
            CancellationToken cancellationToken)
        {
            return await _context.BarberShops
                .Include(x => x.Services)
                .Include(x => x.Barbers)
                .FirstOrDefaultAsync(
                    x => x.OwnerUserId == ownerUserId,
                    cancellationToken);
        }

        public void Add(
            BarberShop barberShop)
        {
            _context.BarberShops.Add(barberShop);
        }

        public void Update(
            BarberShop barberShop)
        {
            _context.BarberShops.Update(barberShop);
        }
    }
}