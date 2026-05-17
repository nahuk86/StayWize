using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.Properties;
using StayWize.Services.ExceptionHandling;
using System.Security.Claims;

namespace StayWize.API.Controllers;

[ApiController]
[Authorize(Roles = "Admin,Owner")]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPropertyRepository _propertyRepository;

    public PropertiesController(IMediator mediator, IPropertyRepository propertyRepository)
    {
        _mediator = mediator;
        _propertyRepository = propertyRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);
        var result = await _mediator.Send(new GetAllPropertiesQuery(userId, role));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetPropertyByIdQuery(id));
        if (result is null) throw new NotFoundException("Propiedad", id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyDto dto)
    {
        // El OwnerId se toma del token, no del body
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userId, out var ownerGuid))
            dto.OwnerId = ownerGuid;

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

    [HttpPost("{id:guid}/assign-hostlocal/{hostLocalId:guid}")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> AssignHostLocal(Guid id, Guid hostLocalId)
    {
        await _propertyRepository.AssignHostLocalAsync(id, hostLocalId);
        return NoContent();
    }

    [HttpDelete("{id:guid}/assign-hostlocal/{hostLocalId:guid}")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> UnassignHostLocal(Guid id, Guid hostLocalId)
    {
        await _propertyRepository.UnassignHostLocalAsync(id, hostLocalId);
        return NoContent();
    }
}