using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.HostLocals;
using StayWize.Services.ExceptionHandling;

namespace StayWize.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class HostLocalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public HostLocalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllHostLocalsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetHostLocalByIdQuery(id));
        if (result is null) throw new NotFoundException("Cliente", id);
        return Ok(result);
    }

    [HttpGet("zone/{zone}")]
    public async Task<IActionResult> GetAvailableByZone(string zone)
    {
        var result = await _mediator.Send(new GetAvailableHostLocalsByZoneQuery(zone));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHostLocalDto dto)
    {
        var result = await _mediator.Send(new CreateHostLocalCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHostLocalDto dto)
    {
        var result = await _mediator.Send(new UpdateHostLocalCommand(id, dto));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id:guid}/availability")]
    public async Task<IActionResult> SetAvailability(Guid id, [FromBody] bool isAvailable)
    {
        var result = await _mediator.Send(new SetHostLocalAvailabilityCommand(id, isAvailable));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteHostLocalCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }
}