using BarberFlow.Application.Features.Appointments.Commands;
using BarberFlow.Application.Features.Appointments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelAppointmentCommand { AppointmentId = id };
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    //[HttpGet("user/{userId}")]
    //public async Task<IActionResult> GetUserAppointments(Guid userId, CancellationToken cancellationToken)
    //{
    //    var query = new GetUserAppointmentsQuery { UserId = userId };
    //    var result = await _mediator.Send(query, cancellationToken);
    //    return Ok(result);
    //}

    [HttpGet("me")]
    public async Task<IActionResult> GetMyAppointments(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUserAppointmentsQuery(), cancellationToken);

        return Ok(result);
    }

    [HttpGet("barbershop/{barberShopId}")]
    public async Task<IActionResult> GetBarberShopAppointments(Guid barberShopId, CancellationToken cancellationToken)
    {
        var query = new GetBarberShopAppointmentsQuery { BarberShopId = barberShopId };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
