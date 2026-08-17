using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories;

public sealed class LocationSearchRepository : ILocationSearchRepository
{
    private readonly BarberFlowDbContext _context;

    public LocationSearchRepository(BarberFlowDbContext context) => _context = context;

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) =>
        _context.LocationSearches.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<LocationSearch>> SearchAsync(string searchText, int take, CancellationToken cancellationToken) =>
        await _context.LocationSearches.AsNoTracking()
            .Where(x => x.SearchText.Contains(searchText))
            .OrderBy(x => x.DisplayName)
            .Take(take)
            .ToListAsync(cancellationToken);
}
