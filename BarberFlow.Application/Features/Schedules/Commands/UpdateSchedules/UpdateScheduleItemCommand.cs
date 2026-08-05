namespace BarberFlow.Application.Features.Schedules.Commands.UpdateSchedules
{
    public class UpdateScheduleItemCommand
    {
        public Guid ScheduleId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }

        public bool IsClosed { get; set; }
    }
}
