using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.AccessCodes;
using StayWize.Application.UseCases.Reservations;
using StayWize.Services.ExceptionHandling;
using System.Security.Claims;

namespace StayWize.API.Controllers;

[ApiController]
[Route("api/access-codes")]
[Authorize]
public class AccessCodesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IClientRepository _clientRepository;

    public AccessCodesController(
        IMediator mediator,
        IPropertyRepository propertyRepository,
        IClientRepository clientRepository)
    {
        _mediator = mediator;
        _propertyRepository = propertyRepository;
        _clientRepository = clientRepository;
    }

    [HttpGet("reservation/{reservationId:guid}")]
    [Authorize(Roles = "Admin,Owner,HostLocal")]
    public async Task<IActionResult> GetByReservation(Guid reservationId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (role != "Admin")
        {
            var reservation = await _mediator.Send(new GetReservationByIdQuery(reservationId));
            if (reservation is null)
                throw new NotFoundException("Reserva", reservationId);

            var properties = role == "Owner"
                ? await _propertyRepository.GetByOwnerIdAsync(userId!)
                : await _propertyRepository.GetByHostLocalUserIdAsync(userId!);

            var propertyIds = properties.Select(p => p.Id);
            if (!propertyIds.Contains(reservation.PropertyId))
                return Forbid();
        }

        var result = await _mediator.Send(new GetAccessCodesByReservationQuery(reservationId));
        return Ok(result);
    }

    /// <summary>
    /// Devuelve los códigos de acceso de las reservas del guest autenticado.
    /// Solo accesible por el rol Guest.
    /// </summary>
    [HttpGet("my-codes")]
    [Authorize(Roles = "Guest")]
    public async Task<IActionResult> GetMyCodes()
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        var client = await _clientRepository.GetByEmailAsync(userEmail!);
        if (client is null)
            return Ok(Enumerable.Empty<AccessCodeDto>());

        var result = await _mediator.Send(new GetAccessCodesByClientQuery(client.Id));
        return Ok(result);
    }

    [HttpGet("{id:guid}/logs")]
    [Authorize(Roles = "Admin,Owner,HostLocal")]
    public async Task<IActionResult> GetLogsByCode(Guid id)
    {
        var result = await _mediator.Send(new GetAccessLogsByCodeQuery(id));
        return Ok(result);
    }

    [HttpGet("reservation/{reservationId:guid}/logs")]
    [Authorize(Roles = "Admin,Owner,HostLocal")]
    public async Task<IActionResult> GetLogsByReservation(Guid reservationId)
    {
        var result = await _mediator.Send(new GetAccessLogsByReservationQuery(reservationId));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Generate([FromBody] GenerateAccessCodeDto dto)
    {
        var result = await _mediator.Send(new GenerateAccessCodeCommand(dto));
        return CreatedAtAction(nameof(GetByReservation),
            new { reservationId = result.ReservationId }, result);
    }

    [HttpPost("validate")]
    [Authorize(Roles = "Admin,Owner,HostLocal,Guest")]
    public async Task<IActionResult> Validate([FromBody] ValidateAccessCodeDto dto)
    {
        var result = await _mediator.Send(new ValidateAccessCodeCommand(dto));
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/revoke")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Revoke(Guid id)
    {
        var result = await _mediator.Send(new RevokeAccessCodeCommand(id));
        if (!result) throw new NotFoundException("Código de acceso", id);
        return NoContent();
    }
}