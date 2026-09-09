using BarberFlow.Domain.Entities;
using BarberFlow.Application.Common.Exceptions;
using Microsoft.Data.SqlClient;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace BarberFlow.Infrastructure.Persistence.Repositories;

public sealed class CollaboratorAvailabilityRepository(BarberFlowDbContext context) : ICollaboratorAvailabilityRepository
{
    public Task<CollaboratorWorkingHours?> GetWorkingHoursAsync(Guid collaboratorId, CancellationToken ct) =>
        context.Set<CollaboratorWorkingHours>().Include(x => x.Periods)
            .SingleOrDefaultAsync(x => x.CollaboratorId == collaboratorId, ct);

    public async Task<IReadOnlyDictionary<Guid, CollaboratorWorkingHours>> GetWorkingHoursAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct) =>
        await context.Set<CollaboratorWorkingHours>().AsNoTracking().Include(x => x.Periods)
            .Where(Matches<CollaboratorWorkingHours>(ids, x => x.CollaboratorId)).ToDictionaryAsync(x => x.CollaboratorId, ct);

    public Task<List<CollaboratorTimeOff>> GetTimeOffAsync(Guid collaboratorId, CancellationToken ct) =>
        context.Set<CollaboratorTimeOff>().Where(x => x.CollaboratorId == collaboratorId
            && x.Status != AvailabilityChangeStatus.Withdrawn
            && x.EndAtUtc > DateTimeOffset.UtcNow)
            .OrderBy(x => x.StartAtUtc).ToListAsync(ct);

    public Task<List<CollaboratorTimeOff>> GetTimeOffAsync(
        Guid collaboratorId,
        DateTimeOffset start,
        DateTimeOffset end,
        CancellationToken ct) =>
        context.Set<CollaboratorTimeOff>().AsNoTracking()
            .Where(x => x.CollaboratorId == collaboratorId)
            .Where(x => x.Status != AvailabilityChangeStatus.Withdrawn)
            .Where(x => x.StartAtUtc < end && x.EndAtUtc > start)
            .OrderBy(x => x.StartAtUtc)
            .ToListAsync(ct);

    public Task<List<CollaboratorTimeOff>> GetTimeOffAsync(IReadOnlyCollection<Guid> ids, DateTimeOffset start, DateTimeOffset end, CancellationToken ct) =>
        context.Set<CollaboratorTimeOff>().AsNoTracking()
            .Where(Matches<CollaboratorTimeOff>(ids, x => x.CollaboratorId))
            .Where(x => x.Status == AvailabilityChangeStatus.Approved && x.StartAtUtc < end && x.EndAtUtc > start).ToListAsync(ct);

    public Task<List<Appointment>> GetFutureAppointmentsAsync(Guid collaboratorId, CancellationToken ct) =>
        context.Appointments.AsNoTracking()
            .Where(x => x.CollaboratorId == collaboratorId && x.Status == AppointmentStatus.Scheduled && x.EndDateTime > DateTimeOffset.UtcNow)
            .OrderBy(x => x.StartDateTime).ToListAsync(ct);

    public void Add(CollaboratorWorkingHours hours) => context.Add(hours);
    public void ReplaceWorkingHours(CollaboratorWorkingHours current, bool useBranchHours, IEnumerable<CollaboratorWorkPeriod> periods)
    {
        context.RemoveRange(current.Periods);
        current.Replace(useBranchHours, periods);
        context.AddRange(current.Periods);
    }
    public Task<List<ProfessionalBusyInterval>> GetBusyIntervalsAsync(IReadOnlyCollection<Guid> ids, DateTimeOffset start, DateTimeOffset end, CancellationToken ct) =>
        (from candidate in context.Collaborators.AsNoTracking().Where(Matches<Collaborator>(ids, x => x.Id))
         from appointment in context.Appointments.AsNoTracking()
         where appointment.Status == AppointmentStatus.Scheduled
            && appointment.StartDateTime < end && appointment.EndDateTime > start
            && (appointment.CollaboratorId == candidate.Id
                || (candidate.UserProfileId != null && appointment.Collaborator.UserProfileId == candidate.UserProfileId))
         select new ProfessionalBusyInterval(candidate.Id, appointment.Id, appointment.StartDateTime, appointment.EndDateTime))
        .ToListAsync(ct);

    public void Add(CollaboratorTimeOff timeOff) => context.Add(timeOff);
    public void Remove(CollaboratorTimeOff timeOff) => context.Remove(timeOff);

    public async Task<IAvailabilityMutation> BeginMutationAsync(IReadOnlyCollection<Guid> collaboratorIds, CancellationToken ct)
    {
        // Share a lock across linked assignments of the same professional.
        // Booking and availability edits hold it until both validation and save finish.
        var identities = await context.Collaborators.AsNoTracking()
            .Where(Matches<Collaborator>(collaboratorIds, x => x.Id))
            .Select(x => x.UserProfileId ?? x.Id).Distinct().ToListAsync(ct);
        var transaction = await context.Database.BeginTransactionAsync(ct);
        try
        {
            foreach (var identity in identities.OrderBy(x => x))
            {
                var resource = $"BarberFlow:professional:{identity:D}";
                await context.Database.ExecuteSqlInterpolatedAsync($"""
                    DECLARE @result int;
                    EXEC @result = sys.sp_getapplock
                        @Resource = {resource}, @LockMode = 'Exclusive',
                        @LockOwner = 'Transaction', @LockTimeout = 10000;
                    IF @result < 0 THROW 51001, 'Professional availability is busy.', 1;
                    """, ct);
            }
            return new AvailabilityMutation(transaction);
        }
        catch (SqlException exception) when (exception.Number == 51001)
        {
            await transaction.DisposeAsync();
            throw new AvailabilityConflictException("La disponibilidad está siendo actualizada. Inténtalo nuevamente.");
        }
        catch
        {
            await transaction.DisposeAsync();
            throw;
        }
    }

    private sealed class AvailabilityMutation(IDbContextTransaction transaction) : IAvailabilityMutation
    {
        public Task CommitAsync(CancellationToken ct) => transaction.CommitAsync(ct);
        public ValueTask DisposeAsync() => transaction.DisposeAsync();
    }

    // Keep these bounded ID filters compatible with the existing SQL Server
    // database, without changing global EF compatibility or requiring OPENJSON.
    private static Expression<Func<T, bool>> Matches<T>(IReadOnlyCollection<Guid> ids, Expression<Func<T, Guid>> key)
    {
        Expression predicate = Expression.Constant(false);
        foreach (var id in ids.Distinct())
            predicate = Expression.OrElse(predicate, Expression.Equal(key.Body, Expression.Constant(id)));
        return Expression.Lambda<Func<T, bool>>(predicate, key.Parameters);
    }
}
