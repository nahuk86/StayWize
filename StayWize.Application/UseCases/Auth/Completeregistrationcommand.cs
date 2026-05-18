using MediatR;
using Microsoft.AspNetCore.Identity;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Domain.Entities;
using StayWize.Services.Authentication;
using StayWize.Services.ExceptionHandling;

namespace StayWize.Application.UseCases.Auth;

public record CompleteRegistrationCommand(CompleteRegistrationDto Dto) : IRequest<AuthResponseDto>;

public class CompleteRegistrationCommandHandler
    : IRequestHandler<CompleteRegistrationCommand, AuthResponseDto>
{
    private readonly IUserInvitationRepository _invitationRepository;
    private readonly IHostLocalRepository _hostLocalRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IAuthService _authService;

    public CompleteRegistrationCommandHandler(
        IUserInvitationRepository invitationRepository,
        IHostLocalRepository hostLocalRepository,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IAuthService authService)
    {
        _invitationRepository = invitationRepository;
        _hostLocalRepository = hostLocalRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(
        CompleteRegistrationCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        if (dto.Password != dto.ConfirmPassword)
            throw new ValidationException("Las contraseñas no coinciden.");

        // Hashear el token recibido para buscar en la base
        var tokenHash = InviteUserCommandHandler.HashToken(dto.Token);
        var invitation = await _invitationRepository.GetByTokenHashAsync(tokenHash);

        if (invitation is null)
            throw new ValidationException("El token de invitación no es válido.");

        if (invitation.IsUsed)
            throw new ValidationException("El token de invitación ya fue utilizado.");

        if (invitation.IsExpired)
            throw new ValidationException("El token de invitación ha expirado.");

        // Crear el usuario en Identity
        var existingUser = await _userManager.FindByEmailAsync(invitation.Email);
        if (existingUser is not null)
            throw new ConflictException($"Ya existe un usuario con el email {invitation.Email}.");

        var user = new AppUser
        {
            FirstName = invitation.FirstName,
            LastName = invitation.LastName,
            Email = invitation.Email,
            UserName = invitation.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
            throw new ValidationException(errors);
        }

        if (!await _roleManager.RoleExistsAsync(invitation.Role))
            await _roleManager.CreateAsync(new IdentityRole(invitation.Role));

        await _userManager.AddToRoleAsync(user, invitation.Role);

        // Si es HostLocal, crear la entidad de dominio automáticamente
        if (invitation.Role == "HostLocal")
        {
            var hostLocal = HostLocal.Create(
                invitation.FirstName,
                invitation.LastName,
                invitation.Email,
                string.Empty,
                string.Empty,
                user.Email!);

            hostLocal.SetUserId(user.Id);
            await _hostLocalRepository.AddAsync(hostLocal);
        }

        // Marcar el token como usado
        invitation.MarkAsUsed();
        await _invitationRepository.UpdateAsync(invitation);

        return await _authService.LoginAsync(new LoginDto
        {
            Email = invitation.Email,
            Password = dto.Password
        });
    }
}