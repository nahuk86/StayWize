using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.Reservations;
using StayWize.Services.ExceptionHandling;
using System.Security.Claims;

namespace StayWize.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin,Owner,HostLocal")]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);
        var result = await _mediator.Send(new GetAllReservationsQuery(userId, role));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetReservationByIdQuery(id));
        if (result is null) throw new NotFoundException("Reserva", id);
        return Ok(result);
    }

    [HttpGet("property/{propertyId:guid}")]
    public async Task<IActionResult> GetByProperty(Guid propertyId)
    {
        var result = await _mediator.Send(new GetReservationsByPropertyQuery(propertyId));
        return Ok(result);
    }

    [HttpGet("client/{clientId:guid}")]
    public async Task<IActionResult> GetByClient(Guid clientId)
    {
        var result = await _mediator.Send(new GetReservationsByClientQuery(clientId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
    {
        var result = await _mediator.Send(new CreateReservationCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}/assign-host")]
    public async Task<IActionResult> AssignHost(Guid id, [FromBody] Guid hostLocalId)
    {
        var result = await _mediator.Send(new AssignHostLocalCommand(id, hostLocalId));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var result = await _mediator.Send(new ConfirmReservationCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _mediator.Send(new CancelReservationCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteReservationCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }
}