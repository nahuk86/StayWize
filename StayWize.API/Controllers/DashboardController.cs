using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayWize.Application.UseCases.Dashboard;

namespace StayWize.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("property/{propertyId:guid}")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> GetPropertyDashboard(
        Guid propertyId,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        var result = await _mediator.Send(
            new GetPropertyDashboardQuery(propertyId, dateFrom, dateTo));
        return Ok(result);
    }

    [HttpGet("global")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetGlobalDashboard(
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        var result = await _mediator.Send(
            new GetGlobalDashboardQuery(dateFrom, dateTo));
        return Ok(result);
    }
}