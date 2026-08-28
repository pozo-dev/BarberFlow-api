using BarberFlow.Application.Features.Client.Appointments;
using BarberFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories.Client;

public sealed class ClientAppointmentRepository : IClientAppointmentRepository
{
    private readonly BarberFlowDbContext _context;

    public ClientAppointmentRepository(BarberFlowDbContext context) => _context = context;

    public async Task<IReadOnlyList<ClientAppointmentRecord>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken) =>
        await _context.Appointments
            .AsNoTracking()
            .Where(appointment => appointment.UserId == clientId)
            .Include(appointment => appointment.Branch).ThenInclude(branch => branch.BarberShop)
            .Include(appointment => appointment.Branch).ThenInclude(branch => branch.LocationSearch)
            .Include(appointment => appointment.Collaborator)
            .Include(appointment => appointment.AppointmentServices).ThenInclude(service => service.Service)
            .OrderBy(appointment => appointment.StartDateTime)
            .Select(appointment => new ClientAppointmentRecord
            {
                Id = appointment.Id,
                BranchId = appointment.BranchId,
                BarberShopName = appointment.Branch.BarberShop.Name,
                BranchName = appointment.Branch.Name,
                BranchAddress = appointment.Branch.Address,
                LocationDisplayName = appointment.Branch.LocationSearch.DisplayName,
                TimeZoneId = appointment.Branch.TimeZoneId,
                ProfessionalName = appointment.Collaborator.FullName,
                StartAtUtc = appointment.StartDateTime,
                EndAtUtc = appointment.EndDateTime,
                Status = (int)appointment.Status,
                Services = appointment.AppointmentServices
                    .OrderBy(service => service.Service.Name)
                    .Select(service => new ClientAppointmentServiceRecord
                    {
                        Id = service.ServiceId,
                        Name = service.Service.Name,
                        Price = service.PriceAtTheMoment,
                    }).ToList(),
            })
            .ToListAsync(cancellationToken);

    public Task<Appointment?> GetOwnedByIdAsync(Guid appointmentId, Guid clientId, CancellationToken cancellationToken) =>
        _context.Appointments
            .Include(appointment => appointment.AppointmentServices)
            .FirstOrDefaultAsync(
            appointment => appointment.Id == appointmentId && appointment.UserId == clientId,
            cancellationToken);
}
