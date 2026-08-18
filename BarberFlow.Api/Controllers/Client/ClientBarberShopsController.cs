using BarberFlow.Application.Features.Client.BarberShops.Queries.GetClientBarberShopById;
using BarberFlow.Application.Features.Client.BarberShops.Queries.GetClientBarberShops;
using BarberFlow.Application.Features.Client.Locations.Queries.GetClientCities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/client/barbershops")]
public sealed class ClientBarberShopsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientBarberShopsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ClientBarberShopPageDto>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? city,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(
            new GetClientBarberShopsQuery(search, city, page, pageSize),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("cities")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetCities(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetClientCitiesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{barberShopId:guid}")]
    public async Task<ActionResult<ClientBarberShopDetailDto>> GetById(
        Guid barberShopId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetClientBarberShopByIdQuery(barberShopId),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{barberShopId:guid}/branches")]
    public async Task<ActionResult<IReadOnlyList<ClientBranchDto>>> GetBranches(Guid barberShopId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetClientBarberShopByIdQuery(barberShopId), cancellationToken);
        return result is null ? NotFound() : Ok(result.Branches);
    }

}
