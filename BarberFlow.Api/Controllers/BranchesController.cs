using BarberFlow.Application.Features.Branches.Commands.AssignBarberToBranch;
using BarberFlow.Application.Features.Branches.Commands.CreateBranch;
using BarberFlow.Application.Features.Branches.Commands.RemoveBarberFromBranch;
using BarberFlow.Application.Features.Branches.Commands.ToggleBranchStatus;
using BarberFlow.Application.Features.Branches.Commands.UpdateBranch;
using BarberFlow.Application.Features.Branches.Queries.GetBranchBarbers;
using BarberFlow.Application.Features.Branches.Queries.GetMyBranches;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers
{
    [ApiController]
    [Route("api/branches")]
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

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(
            [FromBody] CreateBranchCommand command,
            CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(
                command,
                cancellationToken);

            return Ok(id);
        }

        [HttpPut("{id:guid}")]
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

        [HttpDelete("{branchId:guid}/barbers/{profileId:guid}")]
        public async Task<IActionResult> RemoveBarber(
            Guid branchId,
            Guid profileId,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new RemoveBarberFromBranchCommand
                {
                    BranchId = branchId,
                    BarberProfileId = profileId
                },
                cancellationToken);

            return NoContent();
        }
    }
}