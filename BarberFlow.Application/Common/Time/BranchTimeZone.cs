namespace BarberFlow.Application.Common.Time;

/// <summary>
/// Converts between the UTC instants persisted by the system and a branch's
/// commercial time. Schedules deliberately remain DateOnly/TimeOnly because
/// they describe a local business rule, not an instant.
/// </summary>
public static class BranchTimeZone
{
    private const string DefaultTimeZoneId = "America/Managua";

    public static TimeZoneInfo Resolve(string? timeZoneId)
    {
        var id = string.IsNullOrWhiteSpace(timeZoneId) ? DefaultTimeZoneId : timeZoneId;
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(id);
        }
        catch (TimeZoneNotFoundException) when (id == DefaultTimeZoneId)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
        }
    }

    public static DateTimeOffset ToUtc(DateOnly date, TimeOnly time, string? timeZoneId)
    {
        var local = DateTime.SpecifyKind(date.ToDateTime(time), DateTimeKind.Unspecified);
        return new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(local, Resolve(timeZoneId)));
    }

    public static DateTimeOffset ToUtc(DateOnly date, string? timeZoneId) =>
        ToUtc(date, TimeOnly.MinValue, timeZoneId);

    public static DateTimeOffset ToBranchTime(DateTimeOffset utcInstant, string? timeZoneId) =>
        TimeZoneInfo.ConvertTime(utcInstant, Resolve(timeZoneId));
}
