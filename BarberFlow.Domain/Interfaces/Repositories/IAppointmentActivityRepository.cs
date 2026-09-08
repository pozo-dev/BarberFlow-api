using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories;

public interface IAppointmentActivityRepository
{
    void Add(AppointmentActivity activity);
    Task<IReadOnlyList<AppointmentActivity>> GetByAppointmentIdAsync(
        Guid appointmentId,
        CancellationToken cancellationToken);
}
