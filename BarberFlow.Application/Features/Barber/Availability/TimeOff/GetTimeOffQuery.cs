using BarberFlow.Application.Common.Time;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Barber.Availability.TimeOff;

public sealed record TimeOffDto(Guid Id, CollaboratorTimeOffType Type, bool AllDay,
    DateTimeOffset StartAtUtc, DateTimeOffset EndAtUtc, DateTime LocalStart, DateTime LocalEnd, AvailabilityChangeStatus Status, int AffectedAppointments);
public sealed record GetTimeOffQuery(Guid CollaboratorId) : IRequest<IReadOnlyList<TimeOffDto>>;

public sealed class GetTimeOffHandler(BarberAvailabilityAccess access, ICollaboratorAvailabilityRepository repository)
    : IRequestHandler<GetTimeOffQuery, IReadOnlyList<TimeOffDto>>
{
    public async Task<IReadOnlyList<TimeOffDto>> Handle(GetTimeOffQuery request, CancellationToken ct)
    {
        var (_, branch) = await access.ResolveAsync(request.CollaboratorId, ct);
        var items = await repository.GetTimeOffAsync(request.CollaboratorId, ct);
        var appointments = await repository.GetFutureAppointmentsAsync(request.CollaboratorId, ct);
        return items.Select(x => new TimeOffDto(x.Id, x.Type, x.AllDay, x.StartAtUtc, x.EndAtUtc,
            BranchTimeZone.ToBranchTime(x.StartAtUtc, branch.TimeZoneId).DateTime,
            BranchTimeZone.ToBranchTime(x.EndAtUtc, branch.TimeZoneId).DateTime,
            x.Status, x.Status == AvailabilityChangeStatus.Approved ? appointments.Count(a => x.Overlaps(a.StartDateTime, a.EndDateTime)) : 0)).ToList();
    }
}
