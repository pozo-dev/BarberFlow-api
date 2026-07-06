using BarberFlow.Domain.Interfaces;

namespace BarberFlow.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BarberFlowDbContext _context;

        public UnitOfWork(BarberFlowDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
