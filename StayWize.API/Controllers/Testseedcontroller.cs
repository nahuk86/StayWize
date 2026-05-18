using Microsoft.AspNetCore.Mvc;
using StayWize.Services.Authentication;

namespace StayWize.API.Controllers;

/// <summary>
/// Endpoint de seed exclusivo para el entorno de testing.
/// NO debe estar disponible en producción ni desarrollo.
/// </summary>
[ApiController]
[Route("api/test")]
public class TestSeedController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _env;

    public TestSeedController(IAuthService authService, IWebHostEnvironment env)
    {
        _authService = authService;
        _env = env;
    }

    [HttpPost("seed-user")]
    public async Task<IActionResult> SeedUser([FromBody] RegisterDto dto)
    {
        // Doble verificación: solo funciona en entorno Testing
        if (!_env.IsEnvironment("Testing"))
            return NotFound();

        var result = await _authService.RegisterAsync(dto);
        return Ok(result);
    }
}