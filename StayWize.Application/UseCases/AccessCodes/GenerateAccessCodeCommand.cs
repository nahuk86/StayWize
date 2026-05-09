using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Domain.Entities;
using StayWize.Domain.Enums;
using StayWize.Services.Encryption;
using StayWize.Services.ExceptionHandling;

namespace StayWize.Application.UseCases.AccessCodes;

public record GenerateAccessCodeCommand(GenerateAccessCodeDto Dto) : IRequest<AccessCodeDto>;

public class GenerateAccessCodeCommandHandler
    : IRequestHandler<GenerateAccessCodeCommand, AccessCodeDto>
{
    private readonly IAccessCodeRepository _accessCodeRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IEncryptionService _encryptionService;

    public GenerateAccessCodeCommandHandler(
        IAccessCodeRepository accessCodeRepository,
        IReservationRepository reservationRepository,
        IEncryptionService encryptionService)
    {
        _accessCodeRepository = accessCodeRepository;
        _reservationRepository = reservationRepository;
        _encryptionService = encryptionService;
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

        var accessCode = AccessCode.Create(
            dto.ReservationId,
            dto.ValidFrom,
            dto.ValidTo,
            dto.Type);

        // Encriptamos el código antes de persistir
        var encryptedCode = _encryptionService.Encrypt(accessCode.Code);
        accessCode.SetEncryptedCode(encryptedCode);

        await _accessCodeRepository.AddAsync(accessCode);

        return new AccessCodeDto
        {
            Id = accessCode.Id,
            ReservationId = accessCode.ReservationId,
            // Devolvemos el código en claro al cliente
            Code = _encryptionService.Decrypt(accessCode.Code),
            ValidFrom = accessCode.ValidFrom,
            ValidTo = accessCode.ValidTo,
            Status = accessCode.Status,
            Type = accessCode.Type,
            CreatedAt = accessCode.CreatedAt
        };
    }
}