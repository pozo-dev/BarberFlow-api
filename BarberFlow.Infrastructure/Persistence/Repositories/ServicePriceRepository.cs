using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories
{
    public class ServicePriceRepository : IServicePriceRepository
    {
        private readonly BarberFlowDbContext _context;

        public ServicePriceRepository(BarberFlowDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<Guid, decimal>> GetCurrentPricesAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken)
        {
            var prices = await _context.ServicePrices
                .AsNoTracking()
                .Where(sp => ids.Contains(sp.ServiceId) && sp.IsCurrent)
                .Select(sp => new
                {
                    sp.ServiceId,
                    sp.Price
                })
                .ToListAsync(cancellationToken);

            return prices.ToDictionary(p => p.ServiceId, p => p.Price);
        }
    }
}
