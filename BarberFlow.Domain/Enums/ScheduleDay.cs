namespace BarberFlow.Domain.Enums;

public enum ScheduleDay
{
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 7
}

public static class ScheduleDayExtensions
{
    public static ScheduleDay ToScheduleDay(this DayOfWeek dayOfWeek) => dayOfWeek switch
    {
        DayOfWeek.Monday => ScheduleDay.Monday,
        DayOfWeek.Tuesday => ScheduleDay.Tuesday,
        DayOfWeek.Wednesday => ScheduleDay.Wednesday,
        DayOfWeek.Thursday => ScheduleDay.Thursday,
        DayOfWeek.Friday => ScheduleDay.Friday,
        DayOfWeek.Saturday => ScheduleDay.Saturday,
        _ => ScheduleDay.Sunday
    };
}
