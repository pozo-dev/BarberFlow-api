using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories;

public interface IAvailabilityRequestRepository
{
    Task<CollaboratorScheduleRequest?> GetScheduleRequestAsync(Guid id, CancellationToken ct);
    Task<CollaboratorScheduleRequest?> GetLatestScheduleRequestAsync(Guid collaboratorId, CancellationToken ct);
    Task<List<CollaboratorScheduleRequest>> GetOwnerScheduleRequestsAsync(Guid shopId, CancellationToken ct);
    Task<List<CollaboratorTimeOff>> GetOwnerTimeOffAsync(Guid shopId, CancellationToken ct);
    Task<CollaboratorTimeOff?> GetTimeOffByIdAsync(Guid id, CancellationToken ct);
    Task<List<Appointment>> GetAffectedAppointmentsAsync(Guid shopId, CancellationToken ct);
    void Add(CollaboratorScheduleRequest request);
}
