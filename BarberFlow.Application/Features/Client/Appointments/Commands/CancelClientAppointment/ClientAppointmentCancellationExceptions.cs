namespace BarberFlow.Application.Features.Client.Appointments.Commands.CancelClientAppointment;

public sealed class ClientAppointmentNotFoundException : Exception
{
    public ClientAppointmentNotFoundException() : base("No se encontró la cita solicitada.") { }
}

public sealed class ClientAppointmentCannotBeCancelledException : Exception
{
    public ClientAppointmentCannotBeCancelledException() : base("Esta cita ya no puede cancelarse.") { }
}
