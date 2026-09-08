using BarberFlow.Application.Features.Schedules.Commands.UpdateSchedules;
using BarberFlow.Application.Features.Schedules.Queries.GetSchedulesByBranchId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers;
[ApiController]
[Route("api/owner/branches/{branchId:guid}/schedules")]
[Authorize]
public class SchedulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SchedulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Obtiene la configuración semanal completa de una sucursal.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ScheduleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ScheduleDto>>> GetByBranchId(
        Guid branchId,
        CancellationToken cancellationToken)
    {
        var schedules = await _mediator.Send(
            new GetSchedulesByBranchIdQuery { BranchId = branchId },
            cancellationToken);

        return Ok(schedules);
    }

    /// <summary>
    /// Actualiza de forma atómica los siete horarios existentes de una sucursal.
    /// Cada elemento debe incluir su ScheduleId.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(UpdateSchedulesResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateSchedulesResponseDto>> Update(
        Guid branchId,
        [FromBody] UpdateSchedulesCommand command,
        CancellationToken cancellationToken)
    {
        command.BranchId = branchId;

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }
}
