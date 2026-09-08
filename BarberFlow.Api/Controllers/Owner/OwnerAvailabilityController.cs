using BarberFlow.Application.Features.Owner.Availability;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers.Owner;

[ApiController]
[Authorize]
[Route("api/owner")]
public sealed class OwnerAvailabilityController(IMediator mediator) : ControllerBase
{
    [HttpGet("availability")]
    public async Task<ActionResult<OwnerAvailabilityDto>> Get(CancellationToken ct) =>
        Ok(await mediator.Send(new GetOwnerAvailabilityQuery(), ct));

    [HttpPost("collaborators/{collaboratorId:guid}/working-hours/requests/{requestId:guid}/approve")]
    public async Task<IActionResult> ApproveSchedule(
        Guid collaboratorId,
        Guid requestId,
        CancellationToken ct)
    {
        await mediator.Send(
            new ReviewScheduleRequestCommand(
                collaboratorId,
                requestId,
                true),
            ct);
        return NoContent();
    }

    [HttpPost("collaborators/{collaboratorId:guid}/working-hours/requests/{requestId:guid}/reject")]
    public async Task<IActionResult> RejectSchedule(
        Guid collaboratorId,
        Guid requestId,
        CancellationToken ct)
    {
        await mediator.Send(
            new ReviewScheduleRequestCommand(
                collaboratorId,
                requestId,
                false),
            ct);
        return NoContent();
    }

    [HttpPost("collaborators/{collaboratorId:guid}/time-off/{timeOffId:guid}/approve")]
    public async Task<IActionResult> ApproveVacation(
        Guid collaboratorId,
        Guid timeOffId,
        CancellationToken ct)
    {
        await mediator.Send(
            new ReviewTimeOffCommand(
                collaboratorId,
                timeOffId,
                BarberFlow.Domain.Enums.AvailabilityChangeStatus.Approved),
            ct);
        return NoContent();
    }

    [HttpPost("collaborators/{collaboratorId:guid}/time-off/{timeOffId:guid}/reject")]
    public async Task<IActionResult> RejectVacation(
        Guid collaboratorId,
        Guid timeOffId,
        CancellationToken ct)
    {
        await mediator.Send(
            new ReviewTimeOffCommand(
                collaboratorId,
                timeOffId,
                BarberFlow.Domain.Enums.AvailabilityChangeStatus.Rejected),
            ct);
        return NoContent();
    }
}
