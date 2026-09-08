using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories;

public sealed class AppointmentActivityRepository : IAppointmentActivityRepository
{
    private readonly BarberFlowDbContext _context;

    public AppointmentActivityRepository(BarberFlowDbContext context) =>
        _context = context;

    public void Add(AppointmentActivity activity) =>
        _context.AppointmentActivities.Add(activity);

    public async Task<IReadOnlyList<AppointmentActivity>> GetByAppointmentIdAsync(
        Guid appointmentId,
        CancellationToken cancellationToken) =>
        await _context.AppointmentActivities
            .AsNoTracking()
            .Where(activity => activity.AppointmentId == appointmentId)
            .OrderBy(activity => activity.OccurredAtUtc)
            .ToListAsync(cancellationToken);
}
