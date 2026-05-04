using MediatR;
using Microsoft.AspNetCore.Mvc;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.Clients;

namespace StayWize.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllClientsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetClientByIdQuery(id));
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        var result = await _mediator.Send(new GetClientByEmailQuery(email));
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("document/{documentNumber}")]
    public async Task<IActionResult> GetByDocument(string documentNumber)
    {
        var result = await _mediator.Send(new GetClientByDocumentQuery(documentNumber));
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientDto dto)
    {
        var result = await _mediator.Send(new CreateClientCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientDto dto)
    {
        var result = await _mediator.Send(new UpdateClientCommand(id, dto));
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteClientCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }
}