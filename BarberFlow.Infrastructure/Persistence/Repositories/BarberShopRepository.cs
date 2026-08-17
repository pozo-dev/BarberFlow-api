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
                .Include(x => x.Branches)
                    .ThenInclude(x => x.LocationSearch)
                .FirstOrDefaultAsync(
                    x => x.OwnerUserId == ownerUserId,
                    cancellationToken);
        }

        public async Task<IReadOnlyList<BarberShop>> SearchActiveAsync(
            string? search,
            string? city,
            int skip,
            int take,
            CancellationToken cancellationToken)
        {
            return await BuildActiveSearchQuery(search, city)
                .Include(shop => shop.Branches.Where(branch => branch.IsActive))
                    .ThenInclude(branch => branch.LocationSearch)
                .Include(shop => shop.Services.Where(service => service.IsActive))
                .OrderBy(shop => shop.Name)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public Task<int> CountActiveAsync(
            string? search,
            string? city,
            CancellationToken cancellationToken)
        {
            return BuildActiveSearchQuery(search, city).CountAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<string>> GetActiveCitiesAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Branches
                .AsNoTracking()
                .Where(branch => branch.IsActive && branch.BarberShop.IsActive)
                .Select(branch => branch.LocationSearch.DisplayName)
                .Distinct()
                .OrderBy(city => city)
                .ToListAsync(cancellationToken);
        }

        private IQueryable<BarberShop> BuildActiveSearchQuery(string? search, string? city)
        {
            var query = _context.BarberShops
                .AsNoTracking()
                .Where(shop => shop.IsActive && shop.Branches.Any(branch => branch.IsActive));

            if (!string.IsNullOrWhiteSpace(search))
            {
                var value = search.Trim();
                query = query.Where(shop =>
                    shop.Name.Contains(value) ||
                    shop.Description.Contains(value) ||
                    shop.Branches.Any(branch => branch.LocationSearch.DisplayName.Contains(value)));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                var value = city.Trim();
                query = query.Where(shop =>
                    shop.Branches.Any(branch => branch.IsActive && branch.LocationSearch.DisplayName.Contains(value)));
            }

            return query;
        }

        public Task<BarberShop?> GetActiveWithDetailsAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return _context.BarberShops
                .AsNoTracking()
                .Where(shop => shop.Id == id && shop.IsActive)
                .Include(shop => shop.Branches.Where(branch => branch.IsActive))
                    .ThenInclude(branch => branch.Schedules)
                .Include(shop => shop.Branches.Where(branch => branch.IsActive))
                    .ThenInclude(branch => branch.LocationSearch)
                .Include(shop => shop.Services.Where(service => service.IsActive))
                .FirstOrDefaultAsync(cancellationToken);
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
