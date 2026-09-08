using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Appointments.History;
using BarberFlow.Application.Features.Barber.Appointments.Commands.UpdateBarberAppointmentStatus;
using BarberFlow.Domain.Constants;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Barber.Appointments.Queries.GetBarberAppointmentHistory;

public sealed record GetBarberAppointmentHistoryQuery(Guid AppointmentId)
    : IRequest<IReadOnlyList<AppointmentActivityDto>>;

public sealed class GetBarberAppointmentHistoryHandler
    : IRequestHandler<GetBarberAppointmentHistoryQuery, IReadOnlyList<AppointmentActivityDto>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserProfileRepository _profiles;
    private readonly IBarberAppointmentRepository _appointments;
    private readonly IAppointmentActivityRepository _activities;

    public GetBarberAppointmentHistoryHandler(
        ICurrentUserService currentUser,
        IUserProfileRepository profiles,
        IBarberAppointmentRepository appointments,
        IAppointmentActivityRepository activities) =>
        (_currentUser, _profiles, _appointments, _activities) =
            (currentUser, profiles, appointments, activities);

    public async Task<IReadOnlyList<AppointmentActivityDto>> Handle(
        GetBarberAppointmentHistoryQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.ProfileId == Guid.Empty)
            throw new CurrentProfileUnavailableException();

        var profile = await _profiles.GetByIdAsync(_currentUser.ProfileId, cancellationToken);
        if (profile?.RoleId != RoleIds.Barber || !profile.IsActive)
            throw new ForbiddenAccessException();

        var appointment = await _appointments.GetOwnedByIdAsync(
            request.AppointmentId,
            profile.Id,
            cancellationToken);
        if (appointment is null)
            throw new BarberAppointmentNotFoundException();

        var activities = await _activities.GetByAppointmentIdAsync(
            appointment.Id,
            cancellationToken);

        return activities.Select(AppointmentActivityMapper.ToDto).ToList();
    }
}
