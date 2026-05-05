using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Domain.Entities;
using StayWize.Domain.Enums;

namespace StayWize.Application.UseCases.AccessCodes;

public record GenerateAccessCodeCommand(GenerateAccessCodeDto Dto) : IRequest<AccessCodeDto>;

public class GenerateAccessCodeCommandHandler
    : IRequestHandler<GenerateAccessCodeCommand, AccessCodeDto>
{
    private readonly IAccessCodeRepository _accessCodeRepository;
    private readonly IReservationRepository _reservationRepository;

    public GenerateAccessCodeCommandHandler(
        IAccessCodeRepository accessCodeRepository,
        IReservationRepository reservationRepository)
    {
        _accessCodeRepository = accessCodeRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<AccessCodeDto> Handle(
        GenerateAccessCodeCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var reservation = await _reservationRepository.GetByIdAsync(dto.ReservationId);

        if (reservation is null)
            throw new InvalidOperationException($"La reserva {dto.ReservationId} no existe.");

        if (reservation.Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException(
                "Solo se pueden generar códigos para reservas confirmadas.");

        var accessCode = AccessCode.Create(
            dto.ReservationId,
            dto.ValidFrom,
            dto.ValidTo,
            dto.Type);

        await _accessCodeRepository.AddAsync(accessCode);

        return new AccessCodeDto
        {
            Id = accessCode.Id,
            ReservationId = accessCode.ReservationId,
            Code = accessCode.Code,
            ValidFrom = accessCode.ValidFrom,
            ValidTo = accessCode.ValidTo,
            Status = accessCode.Status,
            Type = accessCode.Type,
            CreatedAt = accessCode.CreatedAt
        };
    }
}