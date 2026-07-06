using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly BarberFlowDbContext _context;

        public AppointmentRepository(BarberFlowDbContext context)
        {
            _context = context;
        }

        public void Add(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
        }

        public Task<Appointment?> GetByIdAsync(Guid appointmentId, CancellationToken cancellationToken)
        {
            return _context.Appointments
                .AsNoTracking()
                .Include(a => a.AppointmentServices) // Si tienes navegación
                .FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);
        }

        public Task<bool> ExistsOverlappingAppointmentAsync(Guid barberShopId, DateTime start, DateTime end, CancellationToken cancellationToken)
        {
            return _context.Appointments
                .AsNoTracking()
                .AnyAsync(a =>
                    a.BarberShopId == barberShopId &&
                    a.Status != AppointmentStatus.Cancelled &&
                    (
                        (start < a.EndDateTime && end > a.StartDateTime)
                    ), cancellationToken
                );
        }

        //public void AddAppointmentService(AppointmentService appointmentService)
        //{
        //    _context.AppointmentServices.Add(appointmentService);
        //}

        public Task<List<Appointment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.AppointmentServices) // Si tienes navegación
                .ToListAsync(cancellationToken);
        }

        public Task<List<Appointment>> GetByBarberShopIdAsync(Guid barberShopId, CancellationToken cancellationToken)
        {
            return _context.Appointments
                .Where(a => a.BarberShopId == barberShopId)
                .Include(a => a.AppointmentServices) // Si tienes navegación
                .ToListAsync(cancellationToken);
        }
    }
}
