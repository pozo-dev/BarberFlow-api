using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly BarberFlowDbContext _context;

        public RoleRepository(BarberFlowDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
