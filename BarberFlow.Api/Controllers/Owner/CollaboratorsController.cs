using BarberFlow.Application.Features.Collaborators;
using BarberFlow.Application.Features.Collaborators.Commands.CreateCollaborator;
using BarberFlow.Application.Features.Collaborators.Commands.ToggleCollaboratorStatus;
using BarberFlow.Application.Features.Collaborators.Commands.UpdateCollaborator;
using BarberFlow.Application.Features.Collaborators.Queries.GetCollaboratorById;
using BarberFlow.Application.Features.Collaborators.Queries.GetCollaborators;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers
{
    [ApiController]
    [Route("api/owner/collaborators")]
    [Authorize]
    public class CollaboratorsController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public CollaboratorsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CollaboratorDto>>> GetAll(CancellationToken ct)
        {
            return Ok(await _mediator.Send(new GetCollaboratorsQuery(), ct));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CollaboratorDto>> GetById(
            Guid id,
            CancellationToken ct)
        {
            return Ok(await _mediator.Send(new GetCollaboratorByIdQuery { Id = id }, ct));
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(
            CreateCollaboratorCommand command,
            CancellationToken ct)
        {
            return Ok(await _mediator.Send(command, ct));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            UpdateCollaboratorCommand command,
            CancellationToken ct)
        {
            command.Id = id;
            await _mediator.Send(command, ct);
            return NoContent();
        }

        [HttpPatch("{id:guid}/toggle")]
        public async Task<IActionResult> Toggle(
            Guid id,
            CancellationToken ct)
        {
            await _mediator.Send(new ToggleCollaboratorStatusCommand { Id = id }, ct);
            return NoContent();
        }
    }
}
