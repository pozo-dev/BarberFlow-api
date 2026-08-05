namespace BarberFlow.Domain.Entities
{
    public sealed record BranchScheduleUpdate(
        Guid ScheduleId,
        DayOfWeek DayOfWeek,
        TimeOnly OpenTime,
        TimeOnly CloseTime,
        bool IsClosed);
}
