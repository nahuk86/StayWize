using MediatR;
using Microsoft.AspNetCore.Mvc;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.AccessCodes;

namespace StayWize.API.Controllers;

[ApiController]
[Route("api/access-codes")]
public class AccessCodesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccessCodesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("reservation/{reservationId:guid}")]
    public async Task<IActionResult> GetByReservation(Guid reservationId)
    {
        var result = await _mediator.Send(new GetAccessCodesByReservationQuery(reservationId));
        return Ok(result);
    }

    [HttpGet("{id:guid}/logs")]
    public async Task<IActionResult> GetLogsByCode(Guid id)
    {
        var result = await _mediator.Send(new GetAccessLogsByCodeQuery(id));
        return Ok(result);
    }

    [HttpGet("reservation/{reservationId:guid}/logs")]
    public async Task<IActionResult> GetLogsByReservation(Guid reservationId)
    {
        var result = await _mediator.Send(new GetAccessLogsByReservationQuery(reservationId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] GenerateAccessCodeDto dto)
    {
        var result = await _mediator.Send(new GenerateAccessCodeCommand(dto));
        return CreatedAtAction(nameof(GetByReservation),
            new { reservationId = result.ReservationId }, result);
    }

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] ValidateAccessCodeDto dto)
    {
        var result = await _mediator.Send(new ValidateAccessCodeCommand(dto));
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/revoke")]
    public async Task<IActionResult> Revoke(Guid id)
    {
        var result = await _mediator.Send(new RevokeAccessCodeCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }
}