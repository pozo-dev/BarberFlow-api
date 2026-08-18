using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories
{
    public interface IAppointmentRepository
    {
        void Add(Appointment appointment);
        Task<Appointment?> GetByIdAsync(Guid appointmentId, CancellationToken cancellationToken);
        Task<bool> ExistsOverlappingAppointmentAsync(Guid branchId, Guid collaboratorId, DateTime start, DateTime end, CancellationToken cancellationToken);
        Task<List<Appointment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<List<Appointment>> GetByBarberShopIdAsync(Guid barberShopId, CancellationToken cancellationToken);
        Task<List<Appointment>> GetByBranchAndDateAsync(Guid branchId, DateOnly date, CancellationToken cancellationToken);
        Task<List<Appointment>> GetByBranchAndDateRangeAsync(Guid branchId, DateOnly from, DateOnly to, CancellationToken cancellationToken);
    }
}
