using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.Auth;

namespace StayWize.API.Controllers;

[ApiController]
[Route("api")]
public class InvitationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvitationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Invita a un nuevo Owner, HostLocal o Admin. Solo accesible por Admin.
    /// </summary>
    [HttpPost("admin/invite")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> InviteByAdmin([FromBody] InviteUserDto dto)
    {
        var result = await _mediator.Send(new InviteUserCommand(dto));
        return Ok(result);
    }

    /// <summary>
    /// Invita a un nuevo Guest. Accesible por Admin y Owner.
    /// Solo Admin puede invitar cualquier rol; Owner solo puede invitar Guests.
    /// </summary>
    [HttpPost("owner/invite-guest")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> InviteGuest([FromBody] InviteUserDto dto)
    {
        // Owner solo puede invitar Guests
        var callerRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (callerRole == "Owner" && dto.Role != "Guest")
            return Forbid();

        var result = await _mediator.Send(new InviteUserCommand(dto));
        return Ok(result);
    }
}