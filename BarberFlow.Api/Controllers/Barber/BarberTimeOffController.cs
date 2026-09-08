using BarberFlow.Application.Features.Barber.Availability.TimeOff;
using BarberFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers.Barber;

[ApiController]
[Authorize]
[Route("api/barber/collaborators/{collaboratorId:guid}/time-off")]
public sealed class BarberTimeOffController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TimeOffDto>>> Get(Guid collaboratorId, CancellationToken ct) =>
        Ok(await mediator.Send(new GetTimeOffQuery(collaboratorId), ct));

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(Guid collaboratorId, CreateTimeOffBody body, CancellationToken ct) =>
        Ok(await mediator.Send(new CreateTimeOffCommand(collaboratorId, body.Type, body.AllDay,
            body.StartDate, body.StartTime, body.EndDate, body.EndTime), ct));

    [HttpDelete("{timeOffId:guid}")]
    public async Task<IActionResult> Delete(Guid collaboratorId, Guid timeOffId, CancellationToken ct)
    {
        await mediator.Send(new DeleteTimeOffCommand(collaboratorId, timeOffId), ct);
        return NoContent();
    }
}

public sealed record CreateTimeOffBody(CollaboratorTimeOffType Type, bool AllDay,
    DateOnly StartDate, TimeOnly StartTime, DateOnly EndDate, TimeOnly EndTime);
