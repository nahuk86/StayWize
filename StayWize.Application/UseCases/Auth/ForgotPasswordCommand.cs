using MediatR;
using Microsoft.AspNetCore.Identity;
using StayWize.Application.DTOs;
using StayWize.Services.Authentication;
using StayWize.Services.Notifications;

namespace StayWize.Application.UseCases.Auth;

public record ForgotPasswordCommand(ForgotPasswordDto Dto) : IRequest;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(
        UserManager<AppUser> userManager,
        IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Dto.Email);

        // Siempre respondemos OK aunque el email no exista — evita user enumeration
        if (user is null) return;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // Encode para que viaje seguro en la URL
        var encodedToken = Uri.EscapeDataString(token);

        _ = _emailService.SendPasswordResetAsync(
            user.Email!,
            $"{user.FirstName} {user.LastName}",
            encodedToken);
    }
}