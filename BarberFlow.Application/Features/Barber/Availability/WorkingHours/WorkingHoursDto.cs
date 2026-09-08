using BarberFlow.Domain.Enums;

namespace BarberFlow.Application.Features.Barber.Availability.WorkingHours;

public sealed record WorkPeriodDto(ScheduleDay DayOfWeek, TimeOnly StartTime, TimeOnly EndTime);
public sealed record BranchHoursDto(ScheduleDay DayOfWeek, bool IsClosed, TimeOnly OpenTime, TimeOnly CloseTime);
public sealed record WorkingHoursDto(bool UseBranchHours, string TimeZoneId, DateOnly LocalToday,
    IReadOnlyList<WorkPeriodDto> Periods, IReadOnlyList<BranchHoursDto> BranchHours, ScheduleRequestDto? LatestRequest);

public sealed record ScheduleRequestDto(Guid Id, AvailabilityChangeStatus Status, bool UseBranchHours,
    IReadOnlyList<WorkPeriodDto> Periods, DateTimeOffset CreatedAtUtc);
