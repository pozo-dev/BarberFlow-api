using BarberFlow.Application.Features.Owner.Appointments;
using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories.Owner;

public sealed class OwnerAppointmentRepository : IOwnerAppointmentRepository
{
    private readonly BarberFlowDbContext _context;
    public OwnerAppointmentRepository(BarberFlowDbContext context) => _context = context;
    public async Task<IReadOnlyList<OwnerAppointmentRecord>> GetByBarberShopIdAsync(Guid barberShopId, CancellationToken cancellationToken) => await _context.Appointments.AsNoTracking().Where(item => item.Branch.BarberShopId == barberShopId).OrderBy(item => item.StartDateTime).Select(item => new OwnerAppointmentRecord { Id = item.Id, BranchId = item.BranchId, BranchName = item.Branch.Name, TimeZoneId = item.Branch.TimeZoneId, ClientName = "Cliente", ClientPhoneNumber = item.User.PhoneNumber, CollaboratorId = item.CollaboratorId, CollaboratorName = item.Collaborator.FullName, StartAtUtc = item.StartDateTime, EndAtUtc = item.EndDateTime, Status = (int)item.Status, Services = item.AppointmentServices.OrderBy(service => service.Service.Name).Select(service => new OwnerAppointmentServiceRecord { Name = service.Service.Name, Price = service.PriceAtTheMoment }).ToList() }).ToListAsync(cancellationToken);
    public Task<Appointment?> GetOwnedByIdAsync(Guid appointmentId, Guid barberShopId, CancellationToken cancellationToken) => _context.Appointments.FirstOrDefaultAsync(item => item.Id == appointmentId && item.Branch.BarberShopId == barberShopId, cancellationToken);
}
