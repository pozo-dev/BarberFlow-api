using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly BarberFlowDbContext _context;

        public ServiceRepository(BarberFlowDbContext context)
        {
            _context = context;
        }

        public Task<List<Service>> GetByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken)
        {
            return _context.Services
                .AsNoTracking()
                .Where(s => ids.Contains(s.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Service>> GetByBarberShopIdAsync(
            Guid barberShopId,
            CancellationToken cancellationToken)
        {
            return await _context.Services
                .Where(x => x.BarberShopId == barberShopId)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Service?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            return await _context.Services
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task AddAsync(
            Service service,
            CancellationToken cancellationToken)
        {
            await _context.Services.AddAsync(service, cancellationToken);
        }

        public void Update(Service service)
        {
            _context.Services.Update(service);
        }

        public void Remove(Service service)
        {
            _context.Services.Remove(service);
        }
    }
}
