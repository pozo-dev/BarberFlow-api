using BarberFlow.Application.Features.Barber.Profile.Queries.GetMyBarberProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers.Barber;

[ApiController]
[Authorize]
[Route("api/barber/profile")]
public sealed class BarberProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public BarberProfileController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<BarberProfileDto>> Get(
        CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetMyBarberProfileQuery(), cancellationToken));
}
