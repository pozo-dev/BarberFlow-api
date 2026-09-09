using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories;

public interface ICollaboratorAvailabilityRepository
{
    Task<CollaboratorWorkingHours?> GetWorkingHoursAsync(Guid collaboratorId, CancellationToken ct);
    Task<IReadOnlyDictionary<Guid, CollaboratorWorkingHours>> GetWorkingHoursAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct);
    Task<List<CollaboratorTimeOff>> GetTimeOffAsync(Guid collaboratorId, CancellationToken ct);
    Task<List<CollaboratorTimeOff>> GetTimeOffAsync(Guid collaboratorId, DateTimeOffset start, DateTimeOffset end, CancellationToken ct);
    Task<List<CollaboratorTimeOff>> GetTimeOffAsync(IReadOnlyCollection<Guid> ids, DateTimeOffset start, DateTimeOffset end, CancellationToken ct);
    Task<List<Appointment>> GetFutureAppointmentsAsync(Guid collaboratorId, CancellationToken ct);
    Task<List<ProfessionalBusyInterval>> GetBusyIntervalsAsync(IReadOnlyCollection<Guid> ids, DateTimeOffset start, DateTimeOffset end, CancellationToken ct);
    void Add(CollaboratorWorkingHours hours);
    void ReplaceWorkingHours(CollaboratorWorkingHours current, bool useBranchHours, IEnumerable<CollaboratorWorkPeriod> periods);
    void Add(CollaboratorTimeOff timeOff);
    void Remove(CollaboratorTimeOff timeOff);
    Task<IAvailabilityMutation> BeginMutationAsync(IReadOnlyCollection<Guid> collaboratorIds, CancellationToken ct);
}

public interface IAvailabilityMutation : IAsyncDisposable
{
    Task CommitAsync(CancellationToken ct);
}

public sealed record ProfessionalBusyInterval(Guid CollaboratorId, Guid AppointmentId, DateTimeOffset Start, DateTimeOffset End);
