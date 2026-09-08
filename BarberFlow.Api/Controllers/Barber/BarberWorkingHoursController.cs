using BarberFlow.Application.Features.Barber.Availability.WorkingHours;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers.Barber;

[ApiController]
[Authorize]
[Route("api/barber/collaborators/{collaboratorId:guid}/working-hours")]
public sealed class BarberWorkingHoursController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<WorkingHoursDto>> Get(Guid collaboratorId, CancellationToken ct) =>
        Ok(await mediator.Send(new GetWorkingHoursQuery(collaboratorId), ct));

    [HttpPost("requests")]
    public async Task<ActionResult<Guid>> CreateRequest(Guid collaboratorId, RequestWorkingHoursBody body, CancellationToken ct)
    {
        return Ok(await mediator.Send(new RequestWorkingHoursCommand(collaboratorId, body.UseBranchHours, body.Periods), ct));
    }

    [HttpDelete("requests/{requestId:guid}")]
    public async Task<IActionResult> Withdraw(Guid collaboratorId, Guid requestId, CancellationToken ct)
    {
        await mediator.Send(new WithdrawWorkingHoursRequestCommand(collaboratorId, requestId), ct);
        return NoContent();
    }
}

public sealed record RequestWorkingHoursBody(bool UseBranchHours, IReadOnlyList<WorkPeriodDto> Periods);
