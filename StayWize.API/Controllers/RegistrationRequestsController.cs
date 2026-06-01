using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.RegistrationRequests;

namespace StayWize.API.Controllers;

[ApiController]
[Route("api/registration-requests")]
public class RegistrationRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RegistrationRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateRegistrationRequestDto dto)
    {
        var result = await _mediator.Send(new CreateRegistrationRequestCommand(dto));
        return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _mediator.Send(new GetPendingRegistrationRequestsQuery());
        return Ok(result);
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var result = await _mediator.Send(new ApproveRegistrationRequestCommand(id));
        return Ok(result);
    }
}