using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Common.Time;
using BarberFlow.Domain.Constants;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Barber.Appointments.Queries.GetBarberAppointments;

public sealed record GetBarberAppointmentsQuery(DateOnly Date) : IRequest<IReadOnlyList<BarberAppointmentDto>>;

public sealed class GetBarberAppointmentsHandler : IRequestHandler<GetBarberAppointmentsQuery, IReadOnlyList<BarberAppointmentDto>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserProfileRepository _profiles;
    private readonly IBarberAppointmentRepository _appointments;

    public GetBarberAppointmentsHandler(ICurrentUserService currentUser, IUserProfileRepository profiles, IBarberAppointmentRepository appointments) =>
        (_currentUser, _profiles, _appointments) = (currentUser, profiles, appointments);

    public async Task<IReadOnlyList<BarberAppointmentDto>> Handle(GetBarberAppointmentsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.ProfileId == Guid.Empty) throw new CurrentProfileUnavailableException();
        var profile = await _profiles.GetByIdAsync(_currentUser.ProfileId, cancellationToken);
        if (profile?.RoleId != RoleIds.Barber || !profile.IsActive) throw new ForbiddenAccessException();

        var appointments = await _appointments.GetByBarberProfileIdAsync(profile.Id, cancellationToken);
        return appointments.Select(item => Map(item, request.Date))
            .Where(item => item is not null)
            .Cast<BarberAppointmentDto>()
            .OrderBy(item => item.StartAtUtc)
            .ToList();
    }

    private static BarberAppointmentDto? Map(BarberAppointmentRecord item, DateOnly date)
    {
        var localStart = BranchTimeZone.ToBranchTime(item.StartAtUtc, item.TimeZoneId);
        if (DateOnly.FromDateTime(localStart.DateTime) != date) return null;
        var localEnd = BranchTimeZone.ToBranchTime(item.EndAtUtc, item.TimeZoneId);
        return new BarberAppointmentDto
        {
            Id = item.Id,
            BranchName = item.BranchName,
            ClientPhoneNumber = item.ClientPhoneNumber,
            StartAtUtc = item.StartAtUtc,
            LocalStartTime = TimeOnly.FromDateTime(localStart.DateTime),
            LocalEndTime = TimeOnly.FromDateTime(localEnd.DateTime),
            Status = (AppointmentStatus)item.Status,
            Services = item.Services
        };
    }
}

public sealed class BarberAppointmentDto
{
    public Guid Id { get; init; }
    public string BranchName { get; init; } = string.Empty;
    public string ClientPhoneNumber { get; init; } = string.Empty;
    public DateTimeOffset StartAtUtc { get; init; }
    public TimeOnly LocalStartTime { get; init; }
    public TimeOnly LocalEndTime { get; init; }
    public AppointmentStatus Status { get; init; }
    public IReadOnlyList<string> Services { get; init; } = [];
}
