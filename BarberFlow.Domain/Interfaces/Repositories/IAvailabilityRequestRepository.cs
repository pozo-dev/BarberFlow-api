using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories;

public interface IAvailabilityRequestRepository
{
    Task<CollaboratorScheduleRequest?> GetScheduleRequestAsync(Guid id, CancellationToken ct);
    Task<CollaboratorScheduleRequest?> GetLatestScheduleRequestAsync(Guid collaboratorId, CancellationToken ct);
    Task<List<CollaboratorScheduleRequest>> GetOwnerScheduleRequestsAsync(Guid collaboratorId, DateTimeOffset monthStart, DateTimeOffset monthEnd, CancellationToken ct);
    Task<List<CollaboratorTimeOff>> GetOwnerTimeOffAsync(Guid collaboratorId, DateTimeOffset monthStart, DateTimeOffset monthEnd, CancellationToken ct);
    Task<IReadOnlyDictionary<Guid, int>> GetPendingCountsByCollaboratorAsync(Guid shopId, CancellationToken ct);
    Task<CollaboratorTimeOff?> GetTimeOffByIdAsync(Guid id, CancellationToken ct);
    void Add(CollaboratorScheduleRequest request);
}
