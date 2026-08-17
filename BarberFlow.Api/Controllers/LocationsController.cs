using BarberFlow.Application.Features.Locations.Queries.SearchLocations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers;

[ApiController]
[Route("api/locations")]
[Authorize]
public sealed class LocationsController : ControllerBase
{
    private readonly IMediator _mediator;
    public LocationsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LocationSearchDto>>> Search([FromQuery] string search, CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new SearchLocationsQuery(search), cancellationToken));
}
