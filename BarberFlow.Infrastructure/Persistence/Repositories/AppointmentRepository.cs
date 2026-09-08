using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories;
public class AppointmentRepository : IAppointmentRepository
{
    private readonly BarberFlowDbContext _context;

    public AppointmentRepository(BarberFlowDbContext context)
    {
        _context = context;
    }

    public void Add(Appointment appointment) => _context.Appointments.Add(appointment);

    public Task<Appointment?> GetByIdAsync(Guid appointmentId, CancellationToken cancellationToken) =>
        _context.Appointments
            .Include(a => a.AppointmentServices)
            .FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);

    public Task<List<Appointment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        _context.Appointments
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .Include(a => a.Branch)
            .Include(a => a.AppointmentServices)
            .ToListAsync(cancellationToken);

    public Task<List<Appointment>> GetByBarberShopIdAsync(Guid barberShopId, CancellationToken cancellationToken) =>
        _context.Appointments
            .AsNoTracking()
            .Where(a => a.Branch.BarberShopId == barberShopId)
            .Include(a => a.Branch)
            .Include(a => a.AppointmentServices)
            .ToListAsync(cancellationToken);

}
