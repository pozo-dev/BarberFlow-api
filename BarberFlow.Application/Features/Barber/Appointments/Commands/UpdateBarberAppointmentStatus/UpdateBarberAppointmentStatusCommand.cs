using MediatR;

namespace BarberFlow.Application.Features.Barber.Appointments.Commands.UpdateBarberAppointmentStatus;

public enum BarberAppointmentAction { Complete, NoShow }

public sealed record UpdateBarberAppointmentStatusCommand(Guid AppointmentId, BarberAppointmentAction Action) : IRequest;

public sealed class BarberAppointmentNotFoundException : Exception
{
    public BarberAppointmentNotFoundException() : base("No se encontró una cita asignada a este Barbero.") { }
}

public sealed class BarberAppointmentCannotBeUpdatedException : Exception
{
    public BarberAppointmentCannotBeUpdatedException() : base("Esta cita no puede actualizarse en su estado actual.") { }
}
