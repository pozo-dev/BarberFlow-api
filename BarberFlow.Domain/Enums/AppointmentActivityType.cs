namespace BarberFlow.Domain.Enums;

/// <summary>
/// Immutable business events recorded during an appointment lifecycle.
/// </summary>
public enum AppointmentActivityType
{
    Created = 1,
    Rescheduled = 2,
    Cancelled = 3,
    Completed = 4,
    NoShow = 5,
}
