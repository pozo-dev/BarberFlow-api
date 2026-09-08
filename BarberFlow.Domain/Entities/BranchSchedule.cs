namespace BarberFlow.Domain.Entities;
public class BranchSchedule
{
    public Guid Id { get; private set; }

    public Guid BranchId { get; private set; }

    public BarberFlow.Domain.Enums.ScheduleDay DayOfWeek { get; private set; }

    public TimeOnly OpenTime { get; private set; }

    public TimeOnly CloseTime { get; private set; }

    public bool IsClosed { get; private set; }

    public Branch Branch { get; private set; } = null!;

    private BranchSchedule()
    {
    }

    private BranchSchedule(
        Guid branchId,
        BarberFlow.Domain.Enums.ScheduleDay dayOfWeek,
        TimeOnly openTime,
        TimeOnly closeTime,
        bool isClosed)
    {
        Id = Guid.NewGuid();

        BranchId = branchId;

        DayOfWeek = dayOfWeek;

        OpenTime = openTime;

        CloseTime = closeTime;

        IsClosed = isClosed;
    }

    public static BranchSchedule Create(
        Guid branchId,
        BarberFlow.Domain.Enums.ScheduleDay dayOfWeek,
        TimeOnly openTime,
        TimeOnly closeTime,
        bool isClosed = false)
    {
        if (branchId == Guid.Empty)
            throw new ArgumentException("La sucursal es requerida.");

        ValidateTimes(openTime, closeTime, isClosed);

        return new BranchSchedule(
            branchId,
            dayOfWeek,
            openTime,
            closeTime,
            isClosed);
    }

    public void Update(
        TimeOnly openTime,
        TimeOnly closeTime,
        bool isClosed)
    {
        ValidateTimes(openTime, closeTime, isClosed);

        OpenTime = openTime;

        CloseTime = closeTime;

        IsClosed = isClosed;
    }

    private static void ValidateTimes(
        TimeOnly openTime,
        TimeOnly closeTime,
        bool isClosed)
    {
        if (!isClosed && openTime >= closeTime)
            throw new ArgumentException(
                "La hora de apertura debe ser menor que la hora de cierre.");
    }
}
