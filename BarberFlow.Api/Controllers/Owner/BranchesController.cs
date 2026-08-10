using BarberFlow.Application.Features.Branches.Commands.AssignBarberToBranch;
using BarberFlow.Application.Features.Branches.Commands.CreateBranch;
using BarberFlow.Application.Features.Branches.Commands.RemoveBarberFromBranch;
using BarberFlow.Application.Features.Branches.Commands.ToggleBranchStatus;
using BarberFlow.Application.Features.Branches.Commands.UpdateBranch;
using BarberFlow.Application.Features.Branches.Queries.GetBranchBarbers;
using BarberFlow.Application.Features.Branches.Queries.GetBranchById;
using BarberFlow.Application.Features.Branches.Queries.GetMainBranch;
using BarberFlow.Application.Features.Branches.Queries.GetMyBranches;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers
{
    [ApiController]
    [Route("api/owner/branches")]
    [Authorize]
    public class BranchesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BranchesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<BranchDto>>> GetMyBranches(
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetMyBranchesQuery(),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BranchDetailResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BranchDetailResponseDto>> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetBranchByIdQuery
                {
                    Id = id
                },
                cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> Create(
            [FromBody] CreateBranchCommand command,
            CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(
                command,
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        /// <summary>Actualiza únicamente la información general de la sucursal.</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateBranchCommand command,
            CancellationToken cancellationToken)
        {
            command.Id = id;

            await _mediator.Send(
                command,
                cancellationToken);

            return NoContent();
        }

        [HttpGet("main")]
        public async Task<ActionResult<GetMainBranchResponseDto>> GetMainBranch(
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetMainBranchQuery(),
                cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{id:guid}/toggle")]
        public async Task<IActionResult> Toggle(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new ToggleBranchStatusCommand { Id = id },
                cancellationToken);

            return NoContent();
        }

        [HttpPost("{branchId:guid}/barbers")]
        public async Task<IActionResult> AssignBarber(
            Guid branchId,
            [FromBody] AssignBarberToBranchCommand command,
            CancellationToken cancellationToken)
        {
            command.BranchId = branchId;

            await _mediator.Send(
                command,
                cancellationToken);

            return NoContent();
        }

        [HttpGet("{branchId:guid}/barbers")]
        public async Task<ActionResult<List<BranchBarberDto>>> GetBranchBarbers(
            Guid branchId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetBranchBarbersQuery { BranchId = branchId },
                cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id:guid}/barbers/{profileId:guid}")]
        public async Task<IActionResult> RemoveBarber(
            Guid id,
            Guid profileId,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new RemoveBarberFromBranchCommand
                {
                    Id = id,
                    BarberProfileId = profileId
                },
                cancellationToken);

            return NoContent();
        }
    }
}
