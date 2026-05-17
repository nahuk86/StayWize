using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StayWize.Services.Authentication;

namespace StayWize.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public UsersController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Create([FromBody] RegisterDto dto)
    {
        var callerRole = User.FindFirstValue(ClaimTypes.Role);

        // BE-026-3: Owner solo puede crear Guests
        if (callerRole == "Owner" && dto.Role != "Guest")
            return Forbid();

        var result = await _authService.RegisterAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Email }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
    {
        var callerRole = User.FindFirstValue(ClaimTypes.Role);

        // Owner solo puede actualizar Guests
        if (callerRole == "Owner" && dto.Role != "Guest")
            return Forbid();

        await _userService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
    }
}