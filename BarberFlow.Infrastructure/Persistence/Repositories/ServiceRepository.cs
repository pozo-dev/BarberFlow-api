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

        public Task<List<Service>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken)
        {
            return _context.Services
                .AsNoTracking()
                .Where(s => ids.Contains(s.Id))
                .ToListAsync(cancellationToken);
        }
    }
}
