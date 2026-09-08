using BarberFlow.Domain.Enums;

namespace BarberFlow.Domain.Entities;

public sealed class CollaboratorTimeOff
{
    public Guid Id { get; private set; }
    public Guid CollaboratorId { get; private set; }
    public CollaboratorTimeOffType Type { get; private set; }
    public bool AllDay { get; private set; }
    public DateTimeOffset StartAtUtc { get; private set; }
    public DateTimeOffset EndAtUtc { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public AvailabilityChangeStatus Status { get; private set; }
    public Guid? CreatedByProfileId { get; private set; }
    public Guid? ReviewedByProfileId { get; private set; }
    public DateTimeOffset? ReviewedAtUtc { get; private set; }

    private CollaboratorTimeOff() { }

    public CollaboratorTimeOff(Guid collaboratorId, CollaboratorTimeOffType type, DateTimeOffset startAtUtc, DateTimeOffset endAtUtc, bool allDay = false, Guid? actor = null)
    {
        if (collaboratorId == Guid.Empty || !Enum.IsDefined(type))
            throw new ArgumentException("El colaborador y el tipo de ausencia son requeridos.");
        if (startAtUtc.Offset != TimeSpan.Zero || endAtUtc.Offset != TimeSpan.Zero || endAtUtc <= startAtUtc)
            throw new ArgumentException("El período debe tener un inicio anterior al fin y estar expresado en UTC.");
        Id = Guid.NewGuid();
        CollaboratorId = collaboratorId;
        Type = type;
        AllDay = allDay;
        StartAtUtc = startAtUtc;
        EndAtUtc = endAtUtc;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        CreatedByProfileId = actor;
        Status = type == CollaboratorTimeOffType.Vacation ? AvailabilityChangeStatus.Pending : AvailabilityChangeStatus.Approved;
    }

    public void Decide(AvailabilityChangeStatus decision, Guid actor)
    {
        if (actor == Guid.Empty) throw new ArgumentException("El autor es requerido.");
        if (decision == AvailabilityChangeStatus.Withdrawn)
        {
            if (Status is not (AvailabilityChangeStatus.Pending or AvailabilityChangeStatus.Approved))
                throw new ArgumentException("El registro ya fue gestionado.");
        }
        else if (Status != AvailabilityChangeStatus.Pending || decision is not (AvailabilityChangeStatus.Approved or AvailabilityChangeStatus.Rejected))
            throw new ArgumentException("La solicitud ya fue gestionada.");
        Status = decision;
        ReviewedByProfileId = actor;
        ReviewedAtUtc = DateTimeOffset.UtcNow;
    }

    public bool Overlaps(DateTimeOffset start, DateTimeOffset end) => start < EndAtUtc && end > StartAtUtc;
}
