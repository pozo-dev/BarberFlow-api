namespace BarberFlow.Application.Features.Schedules.Queries.GetSchedulesByBranchId;
public class ScheduleDto
{
    public Guid ScheduleId { get; set; }

    public BarberFlow.Domain.Enums.ScheduleDay DayOfWeek { get; set; }

    public TimeSpan OpenTime { get; set; }

    public TimeSpan CloseTime { get; set; }

    public bool IsClosed { get; set; }
}
