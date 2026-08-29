using MediatR;

namespace BarberFlow.Application.Features.Owner.Appointments.Commands.UpdateOwnerAppointmentStatus;

public enum OwnerAppointmentAction { Complete, NoShow }
public sealed record UpdateOwnerAppointmentStatusCommand(Guid AppointmentId, OwnerAppointmentAction Action) : IRequest;
public sealed class OwnerAppointmentCannotBeUpdatedException : Exception { public OwnerAppointmentCannotBeUpdatedException() : base("Esta cita no puede actualizarse en su estado actual.") { } }
public sealed class OwnerAppointmentNotFoundException : Exception { public OwnerAppointmentNotFoundException() : base("No se encontró la cita solicitada.") { } }
