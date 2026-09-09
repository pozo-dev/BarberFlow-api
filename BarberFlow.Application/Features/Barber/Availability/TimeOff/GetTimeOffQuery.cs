using BarberFlow.Application.Common.Time;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Barber.Availability.TimeOff;

public sealed record TimeOffDto(Guid Id, CollaboratorTimeOffType Type, bool AllDay,
    DateTimeOffset StartAtUtc, DateTimeOffset EndAtUtc, DateTime LocalStart, DateTime LocalEnd, AvailabilityChangeStatus Status, int AffectedAppointments);
public sealed record GetTimeOffQuery(Guid CollaboratorId, int Year, int Month) : IRequest<IReadOnlyList<TimeOffDto>>;

public sealed class GetTimeOffHandler(BarberAvailabilityAccess access, ICollaboratorAvailabilityRepository repository)
    : IRequestHandler<GetTimeOffQuery, IReadOnlyList<TimeOffDto>>
{
    public async Task<IReadOnlyList<TimeOffDto>> Handle(GetTimeOffQuery request, CancellationToken ct)
    {
        if (request.Year is < 2000 or > 2100 || request.Month is < 1 or > 12)
            throw new ArgumentException("El año y el mes son requeridos.");
        var (_, branch) = await access.ResolveAsync(request.CollaboratorId, ct);
        var monthStart = new DateOnly(request.Year, request.Month, 1);
        var nextMonthStart = monthStart.AddMonths(1);
        var startAtUtc = BranchTimeZone.ToUtc(monthStart, branch.TimeZoneId);
        var endAtUtc = BranchTimeZone.ToUtc(nextMonthStart, branch.TimeZoneId);
        var items = await repository.GetTimeOffAsync(
            request.CollaboratorId,
            startAtUtc,
            endAtUtc,
            ct);
        var appointments = await repository.GetFutureAppointmentsAsync(request.CollaboratorId, ct);
        return items.OrderBy(x => x.StartAtUtc).Select(x => new TimeOffDto(x.Id, x.Type, x.AllDay, x.StartAtUtc, x.EndAtUtc,
            BranchTimeZone.ToBranchTime(x.StartAtUtc, branch.TimeZoneId).DateTime,
            BranchTimeZone.ToBranchTime(x.EndAtUtc, branch.TimeZoneId).DateTime,
            x.Status, x.Status == AvailabilityChangeStatus.Approved ? appointments.Count(a => x.Overlaps(a.StartDateTime, a.EndDateTime)) : 0)).ToList();
    }
}
