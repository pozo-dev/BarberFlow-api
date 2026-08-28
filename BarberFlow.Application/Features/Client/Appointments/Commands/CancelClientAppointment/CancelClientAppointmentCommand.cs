using MediatR;

namespace BarberFlow.Application.Features.Client.Appointments.Commands.CancelClientAppointment;

public sealed record CancelClientAppointmentCommand(Guid AppointmentId) : IRequest;
