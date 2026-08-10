using BarberFlow.Api.Models;
using BarberFlow.Api.Validation;
using BarberFlow.Application.Features.BarberShops.Commands.CreateBarberShop;
using BarberFlow.Application.Features.BarberShops.Commands.UpdateBarberShop;
using BarberFlow.Application.Features.BarberShops.Queries.GetMyBarberShop;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers
{
    [ApiController]
    [Route("api/owner/barbershop")]
    [Authorize]
    public class BarberShopController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BarberShopController(
            IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Crea la barbería del usuario autenticado.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CreateBarberShopResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreateBarberShopResponseDto>> Create(
            [FromForm] CreateBarberShopRequestDto request,
            CancellationToken cancellationToken)
        {
            var command = new CreateBarberShopCommand
            {
                Name = request.Name,
                Description = request.Description,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                City = request.City,
                Logo = await BarberShopImageValidator.ReadAndValidateAsync(
                    request.Logo, "El logo", cancellationToken),
                Banner = await BarberShopImageValidator.ReadAndValidateAsync(
                    request.Banner, "El banner", cancellationToken)
            };

            var response = await _mediator.Send(
                command,
                cancellationToken);

            return Ok(response);
        }

        [HttpGet("me")]
        public async Task<ActionResult<GetMyBarberShopResponseDto>> GetMyBarberShop(
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(
                new GetMyBarberShopQuery(),
                cancellationToken);

            return Ok(response);
        }

        [HttpPut]
        [ProducesResponseType(typeof(UpdateBarberShopResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<UpdateBarberShopResponseDto>> Update(
            [FromForm] UpdateBarberShopRequestDto request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateBarberShopCommand
            {
                Name = request.Name,
                Description = request.Description,
                Logo = await BarberShopImageValidator.ReadAndValidateAsync(
                    request.Logo, "El logo", cancellationToken),
                Banner = await BarberShopImageValidator.ReadAndValidateAsync(
                    request.Banner, "El banner", cancellationToken)
            };

            var response =
                await _mediator.Send(
                    command,
                    cancellationToken);

            return Ok(response);
        }
    }
}
