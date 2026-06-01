using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.Auth;
using StayWize.Services.Authentication;

namespace StayWize.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMediator _mediator;

    public AuthController(IAuthService authService, IMediator mediator)
    {
        _authService = authService;
        _mediator = mediator;
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

    [HttpPost("complete-registration")]
    public async Task<IActionResult> CompleteRegistration([FromBody] CompleteRegistrationDto dto)
    {
        var result = await _mediator.Send(new CompleteRegistrationCommand(dto));
        return Ok(result);
    }

    /// <summary>
    /// Solicita un email de recuperación de contraseña.
    /// Siempre responde 200 para evitar user enumeration.
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        await _mediator.Send(new ForgotPasswordCommand(dto));
        return Ok(new { message = "Si el email existe, recibirás un enlace para restablecer tu contraseña." });
    }

    /// <summary>
    /// Restablece la contraseña usando el token recibido por email.
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        await _mediator.Send(new ResetPasswordCommand(dto));
        return Ok(new { message = "Contraseña restablecida correctamente." });
    }
}