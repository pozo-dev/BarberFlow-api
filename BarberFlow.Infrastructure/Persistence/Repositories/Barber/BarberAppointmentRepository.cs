using BarberFlow.Application.Features.Barber.Appointments;
using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories.Barber;

public sealed class BarberAppointmentRepository : IBarberAppointmentRepository
{
    private readonly BarberFlowDbContext _context;

    public BarberAppointmentRepository(BarberFlowDbContext context) => _context = context;

    public async Task<IReadOnlyList<BarberAppointmentRecord>> GetByBarberProfileIdAsync(Guid barberProfileId, CancellationToken cancellationToken) =>
        await _context.Appointments.AsNoTracking()
            .Where(item => item.Collaborator.UserProfileId == barberProfileId && item.Collaborator.IsActive)
            .OrderBy(item => item.StartDateTime)
            .Select(item => new BarberAppointmentRecord
            {
                Id = item.Id,
                BranchName = item.Branch.Name,
                TimeZoneId = item.Branch.TimeZoneId,
                ClientPhoneNumber = item.User.PhoneNumber,
                StartAtUtc = item.StartDateTime,
                EndAtUtc = item.EndDateTime,
                Status = (int)item.Status,
                Services = item.AppointmentServices.OrderBy(service => service.Service.Name).Select(service => service.Service.Name).ToList()
            })
            .ToListAsync(cancellationToken);

    public Task<Appointment?> GetOwnedByIdAsync(Guid appointmentId, Guid barberProfileId, CancellationToken cancellationToken) =>
        _context.Appointments.FirstOrDefaultAsync(
            item => item.Id == appointmentId &&
                    item.Collaborator.UserProfileId == barberProfileId &&
                    item.Collaborator.IsActive,
            cancellationToken);
}
