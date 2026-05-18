using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StayWize.Services.Authentication;
using StayWize.Application.Common.Interfaces;
using StayWize.Domain.Entities;

namespace StayWize.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly IHostLocalRepository _hostLocalRepository;

    public UsersController(
        IUserService userService,
        IAuthService authService,
        IHostLocalRepository hostLocalRepository)
    {
        _userService = userService;
        _authService = authService;
        _hostLocalRepository = hostLocalRepository;
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

    /// <summary>
    /// Crea un usuario con contraseña directa (uso interno / seed).
    /// - Admin puede crear cualquier rol.
    /// - Owner solo puede crear Guests.
    /// Para onboarding de usuarios reales usar POST /api/admin/invite o POST /api/owner/invite-guest.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Create([FromBody] RegisterDto dto)
    {
        var callerRole = User.FindFirstValue(ClaimTypes.Role);

        // Owner solo puede crear Guests
        if (callerRole == "Owner" && dto.Role != "Guest")
            return Forbid();

        // Solo Admin puede crear Owners, HostLocals y otros Admins
        if (callerRole != "Admin" && dto.Role is "Owner" or "HostLocal" or "Admin")
            return Forbid();

        var result = await _authService.RegisterAsync(dto);

        // Si el rol es HostLocal, crear la entidad de dominio automáticamente
        if (dto.Role == "HostLocal")
        {
            var hostLocal = HostLocal.Create(
                dto.FirstName,
                dto.LastName,
                dto.Email,
                string.Empty,
                string.Empty,
                result.Email);

            var users = await _userService.GetAllAsync();
            var newUser = users.FirstOrDefault(u => u.Email == dto.Email);
            if (newUser is not null)
                hostLocal.SetUserId(newUser.Id);

            await _hostLocalRepository.AddAsync(hostLocal);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Email }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Owner")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
    {
        var callerRole = User.FindFirstValue(ClaimTypes.Role);

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