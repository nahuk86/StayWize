using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.Properties;
using StayWize.Services.ExceptionHandling;

namespace StayWize.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin,Owner")]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllPropertiesQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetPropertyByIdQuery(id));
        if (result is null) throw new NotFoundException("Propiedad", id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyDto dto)
    {
        var result = await _mediator.Send(new CreatePropertyCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyDto dto)
    {
        var result = await _mediator.Send(new UpdatePropertyCommand(id, dto));
        if (!result) throw new NotFoundException("Propiedad", id);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeletePropertyCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }
}