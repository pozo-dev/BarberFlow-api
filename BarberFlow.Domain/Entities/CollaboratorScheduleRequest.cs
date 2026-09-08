using BarberFlow.Domain.Enums;

namespace BarberFlow.Domain.Entities;

public sealed class CollaboratorScheduleRequest
{
    public Guid Id { get; private set; }
    public Guid CollaboratorId { get; private set; }
    public bool UseBranchHours { get; private set; }
    public AvailabilityChangeStatus Status { get; private set; }
    public Guid RequestedByProfileId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public Guid? ReviewedByProfileId { get; private set; }
    public DateTimeOffset? ReviewedAtUtc { get; private set; }
    public ICollection<CollaboratorRequestedPeriod> Periods { get; private set; } = new List<CollaboratorRequestedPeriod>();

    private CollaboratorScheduleRequest() { }

    public CollaboratorScheduleRequest(CollaboratorWorkingHours proposal, Guid actor)
    {
        if (actor == Guid.Empty) throw new ArgumentException("El autor es requerido.");
        Id = Guid.NewGuid();
        CollaboratorId = proposal.CollaboratorId;
        UseBranchHours = proposal.UseBranchHours;
        RequestedByProfileId = actor;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        Status = AvailabilityChangeStatus.Pending;
        foreach (var period in proposal.Periods)
            Periods.Add(new CollaboratorRequestedPeriod(period.DayOfWeek, period.StartTime, period.EndTime));
    }

    public void Decide(AvailabilityChangeStatus decision, Guid actor)
    {
        if (Status != AvailabilityChangeStatus.Pending)
            throw new ArgumentException("La solicitud ya fue gestionada.");
        if (actor == Guid.Empty || decision is not (AvailabilityChangeStatus.Approved or AvailabilityChangeStatus.Rejected or AvailabilityChangeStatus.Withdrawn))
            throw new ArgumentException("La decisión no es válida.");
        Status = decision;
        ReviewedByProfileId = actor;
        ReviewedAtUtc = DateTimeOffset.UtcNow;
    }

    public CollaboratorWorkingHours ToWorkingHours() => new(CollaboratorId, UseBranchHours,
        Periods.Select(x => new CollaboratorWorkPeriod(x.DayOfWeek, x.StartTime, x.EndTime)));
}

public sealed class CollaboratorRequestedPeriod
{
    public Guid Id { get; private set; }
    public Guid RequestId { get; private set; }
    public ScheduleDay DayOfWeek { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }

    private CollaboratorRequestedPeriod() { }
    public CollaboratorRequestedPeriod(ScheduleDay day, TimeOnly start, TimeOnly end)
    {
        Id = Guid.NewGuid();
        DayOfWeek = day;
        StartTime = start;
        EndTime = end;
    }
}
