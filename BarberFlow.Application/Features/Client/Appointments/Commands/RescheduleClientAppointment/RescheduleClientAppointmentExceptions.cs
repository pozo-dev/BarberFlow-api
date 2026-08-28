namespace BarberFlow.Application.Features.Client.Appointments.Commands.RescheduleClientAppointment;

public sealed class ClientAppointmentNotFoundForRescheduleException : Exception
{
    public ClientAppointmentNotFoundForRescheduleException() : base("No se encontró la cita solicitada.") { }
}

public sealed class ClientAppointmentCannotBeRescheduledException : Exception
{
    public ClientAppointmentCannotBeRescheduledException() : base("Esta cita ya no puede reprogramarse.") { }
}

public sealed class ClientAppointmentRescheduleConflictException : Exception
{
    public ClientAppointmentRescheduleConflictException() : base("El horario seleccionado ya no está disponible.") { }
}
