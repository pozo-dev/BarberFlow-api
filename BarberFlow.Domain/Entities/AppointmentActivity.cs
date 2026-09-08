using BarberFlow.Domain.Enums;

namespace BarberFlow.Domain.Entities;

/// <summary>
/// An append-only record of a meaningful appointment action.
/// </summary>
public class AppointmentActivity
{
    public Guid Id { get; private set; }
    public Guid AppointmentId { get; private set; }
    public AppointmentActivityType Type { get; private set; }
    public Guid ActorProfileId { get; private set; }
    public int ActorRoleId { get; private set; }
    public DateTimeOffset OccurredAtUtc { get; private set; }
    public DateTimeOffset? PreviousStartAtUtc { get; private set; }
    public DateTimeOffset? NewStartAtUtc { get; private set; }
    public Guid? RelatedAppointmentId { get; private set; }

    public Appointment Appointment { get; private set; } = null!;

    private AppointmentActivity() { }

    public AppointmentActivity(
        Guid appointmentId,
        AppointmentActivityType type,
        Guid actorProfileId,
        int actorRoleId,
        DateTimeOffset? previousStartAtUtc = null,
        DateTimeOffset? newStartAtUtc = null,
        Guid? relatedAppointmentId = null)
    {
        if (appointmentId == Guid.Empty)
            throw new ArgumentException("La cita es requerida.", nameof(appointmentId));

        if (actorProfileId == Guid.Empty)
            throw new ArgumentException("El perfil que realizó la acción es requerido.", nameof(actorProfileId));

        Id = Guid.NewGuid();
        AppointmentId = appointmentId;
        Type = type;
        ActorProfileId = actorProfileId;
        ActorRoleId = actorRoleId;
        OccurredAtUtc = DateTimeOffset.UtcNow;
        PreviousStartAtUtc = previousStartAtUtc;
        NewStartAtUtc = newStartAtUtc;
        RelatedAppointmentId = relatedAppointmentId;
    }
}
