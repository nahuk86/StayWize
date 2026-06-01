using MediatR;
using Microsoft.AspNetCore.Identity;
using StayWize.Application.DTOs;
using StayWize.Services.Authentication;
using StayWize.Services.ExceptionHandling;

namespace StayWize.Application.UseCases.Auth;

public record ResetPasswordCommand(ResetPasswordDto Dto) : IRequest;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly UserManager<AppUser> _userManager;

    public ResetPasswordCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        if (dto.Password != dto.ConfirmPassword)
            throw new ValidationException("Las contraseñas no coinciden.");

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            throw new ValidationException("Los datos ingresados no son válidos.");

        // Decode del token que viajó en la URL
        var decodedToken = Uri.UnescapeDataString(dto.Token);

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
            throw new ValidationException(errors);
        }
    }
}