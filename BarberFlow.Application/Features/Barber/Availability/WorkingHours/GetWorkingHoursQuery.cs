using BarberFlow.Application.Common.Time;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Barber.Availability.WorkingHours;

public sealed record GetWorkingHoursQuery(Guid CollaboratorId) : IRequest<WorkingHoursDto>;

public sealed class GetWorkingHoursHandler(BarberAvailabilityAccess access, ICollaboratorAvailabilityRepository repository, IAvailabilityRequestRepository requests)
    : IRequestHandler<GetWorkingHoursQuery, WorkingHoursDto>
{
    public async Task<WorkingHoursDto> Handle(GetWorkingHoursQuery request, CancellationToken ct)
    {
        var (_, branch) = await access.ResolveAsync(request.CollaboratorId, ct);
        var hours = await repository.GetWorkingHoursAsync(request.CollaboratorId, ct);
        var latest = await requests.GetLatestScheduleRequestAsync(request.CollaboratorId, ct);
        return new WorkingHoursDto(hours?.UseBranchHours ?? true, branch.TimeZoneId,
            DateOnly.FromDateTime(BranchTimeZone.ToBranchTime(DateTimeOffset.UtcNow, branch.TimeZoneId).DateTime),
            hours?.Periods.OrderBy(x => x.DayOfWeek).ThenBy(x => x.StartTime)
                .Select(x => new WorkPeriodDto(x.DayOfWeek, x.StartTime, x.EndTime)).ToList() ?? [],
            branch.Schedules.OrderBy(x => x.DayOfWeek)
                .Select(x => new BranchHoursDto(x.DayOfWeek, x.IsClosed, x.OpenTime, x.CloseTime)).ToList(),
            latest is null ? null : new ScheduleRequestDto(latest.Id, latest.Status, latest.UseBranchHours,
                latest.Periods.Select(x => new WorkPeriodDto(x.DayOfWeek, x.StartTime, x.EndTime)).ToList(), latest.CreatedAtUtc));
    }
}
