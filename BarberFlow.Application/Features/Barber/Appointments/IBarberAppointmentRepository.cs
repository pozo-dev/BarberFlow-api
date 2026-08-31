using BarberFlow.Domain.Entities;

namespace BarberFlow.Application.Features.Barber.Appointments;

public interface IBarberAppointmentRepository
{
    Task<IReadOnlyList<BarberAppointmentRecord>> GetByBarberProfileIdAsync(Guid barberProfileId, CancellationToken cancellationToken);
    Task<Appointment?> GetOwnedByIdAsync(Guid appointmentId, Guid barberProfileId, CancellationToken cancellationToken);
}
  
public sealed class BarberAppointmentRecord
{
    public Guid Id { get; init; }
    public string BranchName { get; init; } = string.Empty;
    public string TimeZoneId { get; init; } = string.Empty;
    public string ClientPhoneNumber { get; init; } = string.Empty;
    public DateTimeOffset StartAtUtc { get; init; }
    public DateTimeOffset EndAtUtc { get; init; }
    public int Status { get; init; }
    public IReadOnlyList<string> Services { get; init; } = [];
}
