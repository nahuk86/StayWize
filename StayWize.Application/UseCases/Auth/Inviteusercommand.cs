using System.Security.Cryptography;
using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Domain.Entities;
using StayWize.Services.ExceptionHandling;
using StayWize.Services.Notifications;

namespace StayWize.Application.UseCases.Auth;

public record InviteUserCommand(InviteUserDto Dto) : IRequest<InvitationResultDto>;

public class InviteUserCommandHandler : IRequestHandler<InviteUserCommand, InvitationResultDto>
{
    private readonly IUserInvitationRepository _invitationRepository;
    private readonly IEmailService _emailService;

    private static readonly string[] ValidRoles = ["Admin", "Owner", "HostLocal", "Guest"];

    public InviteUserCommandHandler(
        IUserInvitationRepository invitationRepository,
        IEmailService emailService)
    {
        _invitationRepository = invitationRepository;
        _emailService = emailService;
    }

    public async Task<InvitationResultDto> Handle(
        InviteUserCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        if (!ValidRoles.Contains(dto.Role))
            throw new ValidationException($"El rol '{dto.Role}' no es válido.");

        // Generar token seguro y su hash para almacenar
        var plainToken = GenerateSecureToken();
        var tokenHash = HashToken(plainToken);

        var invitation = UserInvitation.Create(
            dto.Email,
            dto.FirstName,
            dto.LastName,
            dto.Role,
            tokenHash,
            expirationHours: 48);

        await _invitationRepository.AddAsync(invitation);

        // Enviar email fire-and-forget
        _ = _emailService.SendInvitationAsync(
            dto.Email,
            $"{dto.FirstName} {dto.LastName}",
            dto.Role,
            plainToken,
            invitation.ExpiresAt);

        return new InvitationResultDto
        {
            Email = dto.Email,
            Role = dto.Role,
            ExpiresAt = invitation.ExpiresAt
        };
    }

    public static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    public static string HashToken(string token)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLower();
    }
}