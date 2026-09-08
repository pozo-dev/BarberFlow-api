using BarberFlow.Domain.Enums;

namespace BarberFlow.Domain.Entities;

public sealed class CollaboratorWorkPeriod
{
    public Guid Id { get; private set; }
    public Guid CollaboratorId { get; private set; }
    public ScheduleDay DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }

    private CollaboratorWorkPeriod() { }

    public CollaboratorWorkPeriod(ScheduleDay dayOfWeek, TimeOnly startTime, TimeOnly endTime)
    {
        if (!Enum.IsDefined(dayOfWeek)) throw new ArgumentException("El día debe estar entre lunes (1) y domingo (7).");
        if (startTime >= endTime) throw new ArgumentException("La hora final debe ser posterior a la inicial, dentro del mismo día.");
        Id = Guid.NewGuid();
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
    }
}
