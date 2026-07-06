using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories
{
    public interface IAppointmentRepository
    {
        void Add(Appointment appointment);
        Task<Appointment?> GetByIdAsync(Guid appointmentId, CancellationToken cancellationToken);

        // Nuevo: Verifica si existe un solapamiento de citas en la barbería para el rango dado
        Task<bool> ExistsOverlappingAppointmentAsync(Guid barberShopId, DateTime start, DateTime end, CancellationToken cancellationToken);

        // Nuevo: Agrega un servicio a una cita
        //void AddAppointmentService(AppointmentService appointmentService);

        // Nuevo: Obtiene todas las citas de un usuario
        Task<List<Appointment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        // Nuevo: Obtiene todas las citas de una barbería
        Task<List<Appointment>> GetByBarberShopIdAsync(Guid barberShopId, CancellationToken cancellationToken);

        // (Opcional) Otros métodos que ya tengas definidos...
    }
}
