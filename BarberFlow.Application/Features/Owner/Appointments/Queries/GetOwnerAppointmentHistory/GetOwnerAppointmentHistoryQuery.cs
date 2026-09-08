using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Appointments.History;
using BarberFlow.Application.Features.Owner.Appointments.Commands.UpdateOwnerAppointmentStatus;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Owner.Appointments.Queries.GetOwnerAppointmentHistory;

public sealed record GetOwnerAppointmentHistoryQuery(Guid AppointmentId)
    : IRequest<IReadOnlyList<AppointmentActivityDto>>;

public sealed class GetOwnerAppointmentHistoryHandler
    : IRequestHandler<GetOwnerAppointmentHistoryQuery, IReadOnlyList<AppointmentActivityDto>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUserProfileRepository _profiles;
    private readonly IOwnerAppointmentRepository _appointments;
    private readonly IAppointmentActivityRepository _activities;

    public GetOwnerAppointmentHistoryHandler(
        ICurrentUserService currentUser,
        IUserProfileRepository profiles,
        IOwnerAppointmentRepository appointments,
        IAppointmentActivityRepository activities) =>
        (_currentUser, _profiles, _appointments, _activities) =
            (currentUser, profiles, appointments, activities);

    public async Task<IReadOnlyList<AppointmentActivityDto>> Handle(
        GetOwnerAppointmentHistoryQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.ProfileId == Guid.Empty)
            throw new CurrentProfileUnavailableException();

        var profile = await _profiles.GetByIdAsync(_currentUser.ProfileId, cancellationToken);
        if (profile?.BarberShopId is null)
            throw new ForbiddenAccessException();

        var appointment = await _appointments.GetOwnedByIdAsync(
            request.AppointmentId,
            profile.BarberShopId.Value,
            cancellationToken);
        if (appointment is null)
            throw new OwnerAppointmentNotFoundException();

        var activities = await _activities.GetByAppointmentIdAsync(
            appointment.Id,
            cancellationToken);

        return activities.Select(AppointmentActivityMapper.ToDto).ToList();
    }
}
