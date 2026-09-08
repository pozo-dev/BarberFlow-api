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
    public Task<List<CollaboratorScheduleRequest>> GetOwnerScheduleRequestsAsync(Guid shopId, CancellationToken ct) =>
        context.Set<CollaboratorScheduleRequest>().Include(x => x.Periods)
            .Where(x => context.Collaborators.Any(c => c.Id == x.CollaboratorId && c.Branch.BarberShopId == shopId))
            .Where(x => x.Status == AvailabilityChangeStatus.Pending || x.CreatedAtUtc > DateTimeOffset.UtcNow.AddDays(-30))
            .OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct);
    public Task<List<CollaboratorTimeOff>> GetOwnerTimeOffAsync(Guid shopId, CancellationToken ct) =>
        context.Set<CollaboratorTimeOff>()
            .Where(x => context.Collaborators.Any(c => c.Id == x.CollaboratorId && c.Branch.BarberShopId == shopId))
            .Where(x => x.EndAtUtc > DateTimeOffset.UtcNow)
            .OrderBy(x => x.StartAtUtc).ToListAsync(ct);
    public Task<CollaboratorTimeOff?> GetTimeOffByIdAsync(Guid id, CancellationToken ct) =>
        context.Set<CollaboratorTimeOff>().SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<List<Appointment>> GetAffectedAppointmentsAsync(Guid shopId, CancellationToken ct) =>
        context.Appointments.AsNoTracking().Include(x => x.Branch).Include(x => x.Collaborator).Include(x => x.User)
            .Where(x => x.Branch.BarberShopId == shopId && x.Status == AppointmentStatus.Scheduled && x.EndDateTime > DateTimeOffset.UtcNow)
            .Where(x => context.Set<CollaboratorTimeOff>().Any(t =>
                t.CollaboratorId == x.CollaboratorId && t.Status == AvailabilityChangeStatus.Approved &&
                t.StartAtUtc < x.EndDateTime && t.EndAtUtc > x.StartDateTime))
            .OrderBy(x => x.StartDateTime).ToListAsync(ct);
    public void Add(CollaboratorScheduleRequest request) => context.Add(request);
}
