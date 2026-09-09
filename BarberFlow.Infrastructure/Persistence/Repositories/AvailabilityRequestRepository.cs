using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories;

public sealed class AvailabilityRequestRepository(BarberFlowDbContext context) : IAvailabilityRequestRepository
{
    public Task<CollaboratorScheduleRequest?> GetScheduleRequestAsync(Guid id, CancellationToken ct) =>
        context.Set<CollaboratorScheduleRequest>().Include(x => x.Periods).SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<CollaboratorScheduleRequest?> GetLatestScheduleRequestAsync(Guid collaboratorId, CancellationToken ct) =>
        context.Set<CollaboratorScheduleRequest>().Include(x => x.Periods)
            .Where(x => x.CollaboratorId == collaboratorId).OrderByDescending(x => x.CreatedAtUtc).FirstOrDefaultAsync(ct);
    public Task<List<CollaboratorScheduleRequest>> GetOwnerScheduleRequestsAsync(
        Guid collaboratorId,
        DateTimeOffset monthStart,
        DateTimeOffset monthEnd,
        CancellationToken ct) =>
        context.Set<CollaboratorScheduleRequest>().Include(x => x.Periods)
            .Where(x => x.CollaboratorId == collaboratorId)
            .Where(x => x.Status != AvailabilityChangeStatus.Withdrawn)
            .Where(x => x.Status == AvailabilityChangeStatus.Pending ||
                (x.CreatedAtUtc >= monthStart && x.CreatedAtUtc < monthEnd))
            .OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct);
    public Task<List<CollaboratorTimeOff>> GetOwnerTimeOffAsync(
        Guid collaboratorId,
        DateTimeOffset monthStart,
        DateTimeOffset monthEnd,
        CancellationToken ct) =>
        context.Set<CollaboratorTimeOff>()
            .Where(x => x.CollaboratorId == collaboratorId)
            .Where(x => x.Status != AvailabilityChangeStatus.Withdrawn)
            .Where(x => x.StartAtUtc < monthEnd && x.EndAtUtc > monthStart)
            .OrderBy(x => x.StartAtUtc).ToListAsync(ct);

    public async Task<IReadOnlyDictionary<Guid, int>> GetPendingCountsByCollaboratorAsync(
        Guid shopId,
        CancellationToken ct)
    {
        var scheduleCounts = await context.Set<CollaboratorScheduleRequest>()
            .AsNoTracking()
            .Where(x => x.Status == AvailabilityChangeStatus.Pending)
            .Where(x => context.Collaborators.Any(c =>
                c.Id == x.CollaboratorId && c.Branch.BarberShopId == shopId))
            .GroupBy(x => x.CollaboratorId)
            .Select(group => new { CollaboratorId = group.Key, Count = group.Count() })
            .ToListAsync(ct);
        var timeOffCounts = await context.Set<CollaboratorTimeOff>()
            .AsNoTracking()
            .Where(x => x.Status == AvailabilityChangeStatus.Pending)
            .Where(x => context.Collaborators.Any(c =>
                c.Id == x.CollaboratorId && c.Branch.BarberShopId == shopId))
            .GroupBy(x => x.CollaboratorId)
            .Select(group => new { CollaboratorId = group.Key, Count = group.Count() })
            .ToListAsync(ct);

        return scheduleCounts.Concat(timeOffCounts)
            .GroupBy(item => item.CollaboratorId)
            .ToDictionary(group => group.Key, group => group.Sum(item => item.Count));
    }
    public Task<CollaboratorTimeOff?> GetTimeOffByIdAsync(Guid id, CancellationToken ct) =>
        context.Set<CollaboratorTimeOff>().SingleOrDefaultAsync(x => x.Id == id, ct);
    public void Add(CollaboratorScheduleRequest request) => context.Add(request);
}
