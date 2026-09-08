namespace BarberFlow.Domain.Entities;
public sealed record BranchScheduleUpdate(
    Guid ScheduleId,
    BarberFlow.Domain.Enums.ScheduleDay DayOfWeek,
    TimeOnly OpenTime,
    TimeOnly CloseTime,
    bool IsClosed);
