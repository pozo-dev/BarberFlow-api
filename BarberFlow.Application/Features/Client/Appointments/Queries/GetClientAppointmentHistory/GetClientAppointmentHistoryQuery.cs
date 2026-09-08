using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Appointments.History;
using BarberFlow.Application.Features.Client.Appointments;
using BarberFlow.Application.Features.Client.Appointments.Commands.CancelClientAppointment;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Client.Appointments.Queries.GetClientAppointmentHistory;

public sealed record GetClientAppointmentHistoryQuery(Guid AppointmentId)
    : IRequest<IReadOnlyList<AppointmentActivityDto>>;

public sealed class GetClientAppointmentHistoryHandler
    : IRequestHandler<GetClientAppointmentHistoryQuery, IReadOnlyList<AppointmentActivityDto>>
{
    private readonly IClientAppointmentRepository _appointments;
    private readonly IAppointmentActivityRepository _activities;
    private readonly ICurrentUserService _currentUser;

    public GetClientAppointmentHistoryHandler(
        IClientAppointmentRepository appointments,
        IAppointmentActivityRepository activities,
        ICurrentUserService currentUser) =>
        (_appointments, _activities, _currentUser) = (appointments, activities, currentUser);

    public async Task<IReadOnlyList<AppointmentActivityDto>> Handle(
        GetClientAppointmentHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var appointment = await _appointments.GetOwnedByIdAsync(
            request.AppointmentId,
            _currentUser.UserId,
            cancellationToken);

        if (appointment is null)
            throw new ClientAppointmentNotFoundException();

        var activities = await _activities.GetByAppointmentIdAsync(
            appointment.Id,
            cancellationToken);

        return activities.Select(AppointmentActivityMapper.ToDto).ToList();
    }
}
