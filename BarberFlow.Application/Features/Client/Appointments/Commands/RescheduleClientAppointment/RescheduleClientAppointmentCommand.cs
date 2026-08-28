using MediatR;

namespace BarberFlow.Application.Features.Client.Appointments.Commands.RescheduleClientAppointment;

public sealed record RescheduleClientAppointmentCommand(
    Guid AppointmentId,
    DateTimeOffset StartDateTime,
    Guid? ProfessionalId) : IRequest<Guid>;
