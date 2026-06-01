using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Domain.Entities;
using StayWize.Services.Encryption;
using StayWize.Services.ExceptionHandling;
using StayWize.Services.Notifications;
using StayWize.Application.UseCases.Auth;


namespace StayWize.Application.UseCases.Clients;

public record CreateClientCommand(CreateClientDto Dto) : IRequest<ClientDto>;

// StayWize.Application/UseCases/Clients/CreateClientCommand.cs
public class CreateClientCommandHandler
    : IRequestHandler<CreateClientCommand, ClientDto>
{
    private readonly IClientRepository _repository;
    private readonly IEncryptionService _encryptionService;
    private readonly IUserInvitationRepository _invitationRepository;
    private readonly IEmailService _emailService;

    public CreateClientCommandHandler(
        IClientRepository repository,
        IEncryptionService encryptionService,
        IUserInvitationRepository invitationRepository,
        IEmailService emailService)
    {
        _repository = repository;
        _encryptionService = encryptionService;
        _invitationRepository = invitationRepository;
        _emailService = emailService;
    }

    public async Task<ClientDto> Handle(
        CreateClientCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Validaciones de unicidad — sin cambios
        var existingEmail = await _repository.GetByEmailAsync(dto.Email);
        if (existingEmail is not null)
            throw new ConflictException($"Ya existe un cliente con el email {dto.Email}.");

        var existingDocument = await _repository.GetByDocumentNumberAsync(dto.DocumentNumber);
        if (existingDocument is not null)
            throw new ConflictException($"Ya existe un cliente con el documento {dto.DocumentNumber}.");

        // Persistencia — sin cambios
        var encryptedDocument = _encryptionService.Encrypt(dto.DocumentNumber);
        var client = Client.Create(
            dto.FirstName, dto.LastName, dto.Email,
            dto.Phone ?? string.Empty, encryptedDocument);

        await _repository.AddAsync(client);

        // ── NUEVO: generar invitación one-time para completar contraseña ──
        var plainToken = InviteUserCommandHandler.GenerateSecureToken();
        var tokenHash  = InviteUserCommandHandler.HashToken(plainToken);

        var invitation = UserInvitation.Create(
            dto.Email,
            dto.FirstName,
            dto.LastName,
            role: "Guest",
            tokenHash,
            expirationHours: 72);  // 3 días para huéspedes

        await _invitationRepository.AddAsync(invitation);

        // Fire-and-forget — igual que el flujo de Owner
        _ = _emailService.SendInvitationAsync(
            dto.Email,
            $"{dto.FirstName} {dto.LastName}",
            role: "Guest",
            plainToken,
            invitation.ExpiresAt);

        return new ClientDto
        {
            Id             = client.Id,
            FirstName      = client.FirstName,
            LastName       = client.LastName,
            Email          = client.Email,
            Phone          = client.Phone,
            DocumentNumber = _encryptionService.Decrypt(client.DocumentNumber),
            CreatedAt      = client.CreatedAt,
            UpdatedAt      = client.UpdatedAt
        };
    }
}