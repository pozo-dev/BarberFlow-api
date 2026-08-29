using BarberFlow.Application.Features.Owner.Appointments.Commands.UpdateOwnerAppointmentStatus;
using BarberFlow.Application.Features.Owner.Appointments.Queries.GetOwnerAppointments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers.Owner;

[ApiController]
[Authorize]
[Route("api/owner/appointments")]
public sealed class OwnerAppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public OwnerAppointmentsController(IMediator mediator) => _mediator = mediator;
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OwnerAppointmentDto>>> Get([FromQuery] DateOnly date, [FromQuery] Guid? branchId, [FromQuery] Guid? collaboratorId, CancellationToken cancellationToken) => Ok(await _mediator.Send(new GetOwnerAppointmentsQuery(date, branchId, collaboratorId), cancellationToken));
    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct) { await _mediator.Send(new UpdateOwnerAppointmentStatusCommand(id, OwnerAppointmentAction.Complete), ct); return NoContent(); }
    [HttpPatch("{id:guid}/no-show")]
    public async Task<IActionResult> NoShow(Guid id, CancellationToken ct) { await _mediator.Send(new UpdateOwnerAppointmentStatusCommand(id, OwnerAppointmentAction.NoShow), ct); return NoContent(); }
}
