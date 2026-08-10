using BarberFlow.Application.Features.Services.Commands.CreateService;
using BarberFlow.Application.Features.Services.Commands.DeleteService;
using BarberFlow.Application.Features.Services.Commands.ToggleServiceStatus;
using BarberFlow.Application.Features.Services.Commands.UpdateService;
using BarberFlow.Application.Features.Services.Queries.GetMyServices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers
{
    [ApiController]
    [Route("api/owner/services")]
    [Authorize]
    public class ServicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ServicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<ServiceDto>>> GetMyServices(
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetMyServicesQuery(),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(
            CreateServiceCommand command,
            CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(command, cancellationToken);
            return Ok(id);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            UpdateServiceCommand command,
            CancellationToken cancellationToken)
        {
            command.Id = id;
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPatch("{id:guid}/toggle")]
        public async Task<IActionResult> Toggle(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new ToggleServiceStatusCommand { Id = id },
                cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new DeleteServiceCommand { Id = id },
                cancellationToken);

            return NoContent();
        }
    }
}
