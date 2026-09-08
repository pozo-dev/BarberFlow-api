using BarberFlow.Application.Features.Barber.Appointments.Commands.UpdateBarberAppointmentStatus;
using BarberFlow.Application.Features.Barber.Appointments.Queries.GetBarberAppointments;
using BarberFlow.Application.Features.Barber.Appointments.Queries.GetBarberAppointmentHistory;
using BarberFlow.Application.Features.Appointments.History;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers.Barber;

[ApiController]
[Authorize]
[Route("api/barber/appointments")]
public sealed class BarberAppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BarberAppointmentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BarberAppointmentDto>>> Get(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetBarberAppointmentsQuery(from, to), cancellationToken));

    [HttpGet("{id:guid}/history")]
    public async Task<ActionResult<IReadOnlyList<AppointmentActivityDto>>> GetHistory(
        Guid id,
        CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetBarberAppointmentHistoryQuery(id), cancellationToken));

    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateBarberAppointmentStatusCommand(id, BarberAppointmentAction.Complete), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/no-show")]
    public async Task<IActionResult> NoShow(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateBarberAppointmentStatusCommand(id, BarberAppointmentAction.NoShow), cancellationToken);
        return NoContent();
    }
}
