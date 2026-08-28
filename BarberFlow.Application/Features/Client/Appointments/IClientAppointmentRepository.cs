using BarberFlow.Domain.Entities;

namespace BarberFlow.Application.Features.Client.Appointments;

public interface IClientAppointmentRepository
{
    Task<IReadOnlyList<ClientAppointmentRecord>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken);
    Task<Appointment?> GetOwnedByIdAsync(Guid appointmentId, Guid clientId, CancellationToken cancellationToken);
}

public sealed class ClientAppointmentRecord
{
    public Guid Id { get; init; }
    public Guid BranchId { get; init; }
    public string BarberShopName { get; init; } = string.Empty;
    public string BranchName { get; init; } = string.Empty;
    public string BranchAddress { get; init; } = string.Empty;
    public string LocationDisplayName { get; init; } = string.Empty;
    public string TimeZoneId { get; init; } = string.Empty;
    public string ProfessionalName { get; init; } = string.Empty;
    public DateTimeOffset StartAtUtc { get; init; }
    public DateTimeOffset EndAtUtc { get; init; }
    public int Status { get; init; }
    public IReadOnlyList<ClientAppointmentServiceRecord> Services { get; init; } = [];
}

public sealed class ClientAppointmentServiceRecord
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
}
