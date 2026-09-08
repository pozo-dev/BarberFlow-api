using BarberFlow.Application.Features.Appointments.Commands;
using BarberFlow.Application.Features.Appointments.DTOs;
using BarberFlow.Application.Features.Client.Appointments.Commands.CancelClientAppointment;
using BarberFlow.Application.Features.Client.Appointments.Commands.RescheduleClientAppointment;
using BarberFlow.Application.Features.Client.Appointments.Queries.GetAppointmentAvailability;
using BarberFlow.Application.Features.Client.Appointments.Queries.GetMyAppointments;
using BarberFlow.Application.Features.Client.Appointments.Queries.GetClientAppointmentHistory;
using BarberFlow.Application.Features.Appointments.History;
using BarberFlow.Application.Features.Client.Appointments.Queries.GetAvailableProfessionals;
using BarberFlow.Application.Features.Client.Appointments.Queries.GetBranchBookingData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberFlow.Api.Controllers.Client;

[ApiController]
[Authorize]
[Route("api/client")]
public sealed class ClientAppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ClientAppointmentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("branches/{branchId:guid}/services")]
    public async Task<ActionResult<ClientBranchBookingDataDto>> GetServices(Guid branchId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBranchBookingDataQuery(branchId), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("branches/{branchId:guid}/professionals")]
    public async Task<ActionResult<IReadOnlyList<ClientProfessionalDto>>> GetProfessionals(Guid branchId, CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAvailableProfessionalsQuery(branchId), cancellationToken));

    [HttpGet("appointment-availability")]
    public async Task<ActionResult<ClientAppointmentAvailabilityDto>> GetAvailability([FromQuery] Guid branchId, [FromQuery] DateOnly date, [FromQuery] List<Guid> serviceIds, [FromQuery] Guid? professionalId, CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetAppointmentAvailabilityQuery(branchId, date, serviceIds, professionalId), cancellationToken));

    [HttpGet("appointment-availability/calendar")]
    public async Task<ActionResult<ClientAppointmentAvailabilityCalendarDto>> GetAvailabilityCalendar([FromQuery] Guid branchId, [FromQuery] DateOnly from, [FromQuery] DateOnly to, [FromQuery] List<Guid> serviceIds, [FromQuery] Guid? professionalId, CancellationToken cancellationToken)
    {
        if (to < from || to.DayNumber - from.DayNumber > 31)
            return BadRequest("El período de disponibilidad debe tener entre uno y 32 días.");
        return Ok(await _mediator.Send(new GetAppointmentAvailabilityCalendarQuery(branchId, from, to, serviceIds, professionalId), cancellationToken));
    }

    [HttpPost("appointments")]
    public async Task<ActionResult<AppointmentDto>> Create([FromBody] CreateAppointmentCommand command, CancellationToken cancellationToken)
    {
        var appointment = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpGet("appointments")]
    public async Task<ActionResult<IReadOnlyList<ClientAppointmentDto>>> GetMine(CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetMyAppointmentsQuery(), cancellationToken));

    [HttpGet("appointments/{id:guid}")]
    public async Task<ActionResult<ClientAppointmentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var appointments = await _mediator.Send(new GetMyAppointmentsQuery(), cancellationToken);
        var appointment = appointments.SingleOrDefault(item => item.Id == id);
        return appointment is null ? NotFound() : Ok(appointment);
    }

    [HttpGet("appointments/{id:guid}/history")]
    public async Task<ActionResult<IReadOnlyList<AppointmentActivityDto>>> GetHistory(
        Guid id,
        CancellationToken cancellationToken) =>
        Ok(await _mediator.Send(new GetClientAppointmentHistoryQuery(id), cancellationToken));

    [HttpPost("appointments/{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CancelClientAppointmentCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPut("appointments/{id:guid}/reschedule")]
    public async Task<ActionResult<Guid>> Reschedule(Guid id, [FromBody] RescheduleClientAppointmentCommand command, CancellationToken cancellationToken)
    {
        var replacementId = await _mediator.Send(command with { AppointmentId = id }, cancellationToken);
        return Ok(replacementId);
    }
}
