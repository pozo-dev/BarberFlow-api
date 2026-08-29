using BarberFlow.Domain.Entities;

namespace BarberFlow.Application.Features.Owner.Appointments;

public interface IOwnerAppointmentRepository
{
    Task<IReadOnlyList<OwnerAppointmentRecord>> GetByBarberShopIdAsync(Guid barberShopId, CancellationToken cancellationToken);
    Task<Appointment?> GetOwnedByIdAsync(Guid appointmentId, Guid barberShopId, CancellationToken cancellationToken);
}

public sealed class OwnerAppointmentRecord
{
    public Guid Id { get; init; }
    public Guid BranchId { get; init; }
    public string BranchName { get; init; } = string.Empty;
    public string TimeZoneId { get; init; } = string.Empty;
    public string ClientName { get; init; } = string.Empty;
    public string ClientPhoneNumber { get; init; } = string.Empty;
    public Guid CollaboratorId { get; init; }
    public string CollaboratorName { get; init; } = string.Empty;
    public DateTimeOffset StartAtUtc { get; init; }
    public DateTimeOffset EndAtUtc { get; init; }
    public int Status { get; init; }
    public IReadOnlyList<OwnerAppointmentServiceRecord> Services { get; init; } = [];
}
public sealed class OwnerAppointmentServiceRecord { public string Name { get; init; } = string.Empty; public decimal Price { get; init; } }
