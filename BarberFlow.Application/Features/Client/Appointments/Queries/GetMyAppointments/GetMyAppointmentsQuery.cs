using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Common.Time;
using BarberFlow.Domain.Enums;
using MediatR;

namespace BarberFlow.Application.Features.Client.Appointments.Queries.GetMyAppointments;

public sealed record GetMyAppointmentsQuery : IRequest<IReadOnlyList<ClientAppointmentDto>>;

public sealed class GetMyAppointmentsHandler : IRequestHandler<GetMyAppointmentsQuery, IReadOnlyList<ClientAppointmentDto>>
{
    private readonly IClientAppointmentRepository _appointments;
    private readonly ICurrentUserService _currentUser;

    public GetMyAppointmentsHandler(IClientAppointmentRepository appointments, ICurrentUserService currentUser)
    {
        _appointments = appointments;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<ClientAppointmentDto>> Handle(GetMyAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _appointments.GetByClientIdAsync(_currentUser.UserId, cancellationToken);
        return appointments
            .Select(appointment =>
            {
                var localStart = BranchTimeZone.ToBranchTime(appointment.StartAtUtc, appointment.TimeZoneId);
                var localEnd = BranchTimeZone.ToBranchTime(appointment.EndAtUtc, appointment.TimeZoneId);
                return new ClientAppointmentDto
                {
                    Id = appointment.Id,
                    BranchId = appointment.BranchId,
                    BarberShopName = appointment.BarberShopName,
                    BranchName = appointment.BranchName,
                    BranchAddress = appointment.BranchAddress,
                    LocationDisplayName = appointment.LocationDisplayName,
                    ProfessionalName = appointment.ProfessionalName,
                    StartAtUtc = appointment.StartAtUtc,
                    EndAtUtc = appointment.EndAtUtc,
                    LocalDate = DateOnly.FromDateTime(localStart.DateTime),
                    LocalStartTime = TimeOnly.FromDateTime(localStart.DateTime),
                    LocalEndTime = TimeOnly.FromDateTime(localEnd.DateTime),
                    Status = (AppointmentStatus)appointment.Status,
                    CanCancel = appointment.Status == (int)AppointmentStatus.Scheduled && appointment.StartAtUtc > DateTimeOffset.UtcNow,
                    CanReschedule = appointment.Status == (int)AppointmentStatus.Scheduled && appointment.StartAtUtc > DateTimeOffset.UtcNow,
                    Services = appointment.Services.Select(service => new ClientAppointmentServiceDto
                    {
                        Id = service.Id,
                        Name = service.Name,
                        Price = service.Price,
                    }).ToList(),
                };
            })
            .OrderBy(appointment => appointment.StartAtUtc)
            .ToList();
    }
}

public sealed class ClientAppointmentDto
{
    public Guid Id { get; init; }
    public Guid BranchId { get; init; }
    public string BarberShopName { get; init; } = string.Empty;
    public string BranchName { get; init; } = string.Empty;
    public string BranchAddress { get; init; } = string.Empty;
    public string LocationDisplayName { get; init; } = string.Empty;
    public string ProfessionalName { get; init; } = string.Empty;
    public DateTimeOffset StartAtUtc { get; init; }
    public DateTimeOffset EndAtUtc { get; init; }
    public DateOnly LocalDate { get; init; }
    public TimeOnly LocalStartTime { get; init; }
    public TimeOnly LocalEndTime { get; init; }
    public AppointmentStatus Status { get; init; }
    public bool CanCancel { get; init; }
    public bool CanReschedule { get; init; }
    public IReadOnlyList<ClientAppointmentServiceDto> Services { get; init; } = [];
}

public sealed class ClientAppointmentServiceDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
}
