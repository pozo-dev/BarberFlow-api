using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories
{
    public class CollaboratorRepository : ICollaboratorRepository
    {
        private readonly BarberFlowDbContext _context;
        public CollaboratorRepository(BarberFlowDbContext context) => _context = context;

        public Task<List<Collaborator>> GetByBarberShopIdAsync(Guid barberShopId, CancellationToken cancellationToken) =>
            _context.Collaborators
                .Include(x => x.Branch)
                .Where(x => x.Branch.BarberShopId == barberShopId)
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);

        public Task<Collaborator?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
            _context.Collaborators
                .Include(x => x.Branch)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task AddAsync(Collaborator collaborator, CancellationToken cancellationToken)
        {
            await _context.Collaborators.AddAsync(collaborator, cancellationToken);
        }
    }
}
