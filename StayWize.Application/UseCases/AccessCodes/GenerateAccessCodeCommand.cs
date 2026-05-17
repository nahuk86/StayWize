using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Domain.Entities;
using StayWize.Domain.Enums;
using StayWize.Services.Encryption;
using StayWize.Services.ExceptionHandling;
using StayWize.Services.Notifications;

namespace StayWize.Application.UseCases.AccessCodes;

public record GenerateAccessCodeCommand(GenerateAccessCodeDto Dto) : IRequest<AccessCodeDto>;

public class GenerateAccessCodeCommandHandler
    : IRequestHandler<GenerateAccessCodeCommand, AccessCodeDto>
{
    private readonly IAccessCodeRepository _accessCodeRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly IEmailService _emailService;

    public GenerateAccessCodeCommandHandler(
        IAccessCodeRepository accessCodeRepository,
        IReservationRepository reservationRepository,
        IPropertyRepository propertyRepository,
        IClientRepository clientRepository,
        IEncryptionService encryptionService,
        IEmailService emailService)
    {
        _accessCodeRepository = accessCodeRepository;
        _reservationRepository = reservationRepository;
        _propertyRepository = propertyRepository;
        _clientRepository = clientRepository;
        _encryptionService = encryptionService;
        _emailService = emailService;
    }

    public async Task<AccessCodeDto> Handle(
        GenerateAccessCodeCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var reservation = await _reservationRepository.GetByIdAsync(dto.ReservationId);
        if (reservation is null)
            throw new NotFoundException("Reserva", dto.ReservationId);

        if (reservation.Status != ReservationStatus.Confirmed)
            throw new ConflictException("Solo se pueden generar códigos para reservas confirmadas.");

        // Regla de negocio: solo se generan códigos para propiedades con self check-in
        var property = await _propertyRepository.GetByIdAsync(reservation.PropertyId);
        if (property is null)
            throw new NotFoundException("Propiedad", reservation.PropertyId);

        if (!property.IsSelfCheckIn)
            throw new ConflictException(
                "Solo se pueden generar códigos de acceso para propiedades con self check-in habilitado.");

        var accessCode = AccessCode.Create(
            dto.ReservationId,
            dto.ValidFrom,
            dto.ValidTo,
            dto.Type);

        var plainCode = accessCode.Code;
        var encryptedCode = _encryptionService.Encrypt(plainCode);
        accessCode.SetEncryptedCode(encryptedCode);

        await _accessCodeRepository.AddAsync(accessCode);

        // Notificación al guest — fire and forget
        var client = await _clientRepository.GetByIdAsync(reservation.ClientId);
        if (client is not null)
        {
            _ = _emailService.SendAccessCodeGeneratedAsync(
                client.Email,
                $"{client.FirstName} {client.LastName}",
                plainCode,
                accessCode.ValidFrom,
                accessCode.ValidTo);
        }

        return new AccessCodeDto
        {
            Id = accessCode.Id,
            ReservationId = accessCode.ReservationId,
            Code = plainCode,
            ValidFrom = accessCode.ValidFrom,
            ValidTo = accessCode.ValidTo,
            Status = accessCode.Status,
            Type = accessCode.Type,
            CreatedAt = accessCode.CreatedAt
        };
    }
}