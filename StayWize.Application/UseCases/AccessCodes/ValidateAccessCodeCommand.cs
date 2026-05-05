using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Domain.Entities;

namespace StayWize.Application.UseCases.AccessCodes;

public record ValidateAccessCodeCommand(ValidateAccessCodeDto Dto)
    : IRequest<ValidateAccessCodeResultDto>;

public class ValidateAccessCodeCommandHandler
    : IRequestHandler<ValidateAccessCodeCommand, ValidateAccessCodeResultDto>
{
    private readonly IAccessCodeRepository _accessCodeRepository;
    private readonly IAccessLogRepository _accessLogRepository;

    public ValidateAccessCodeCommandHandler(
        IAccessCodeRepository accessCodeRepository,
        IAccessLogRepository accessLogRepository)
    {
        _accessCodeRepository = accessCodeRepository;
        _accessLogRepository = accessLogRepository;
    }

    public async Task<ValidateAccessCodeResultDto> Handle(
        ValidateAccessCodeCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var accessCode = await _accessCodeRepository.GetByCodeAsync(dto.Code);

        // Código no encontrado
        if (accessCode is null)
        {
            return new ValidateAccessCodeResultDto
            {
                Success = false,
                FailureReason = "Código de acceso no encontrado."
            };
        }

        // Código inválido (vencido, revocado, fuera de rango)
        if (!accessCode.IsValid())
        {
            var log = AccessLog.CreateFailure(
                accessCode.Id,
                accessCode.ReservationId,
                dto.EventType,
                $"Código inválido. Estado: {accessCode.Status}.");

            await _accessLogRepository.AddAsync(log);

            return new ValidateAccessCodeResultDto
            {
                Success = false,
                FailureReason = $"Código inválido. Estado: {accessCode.Status}.",
                ReservationId = accessCode.ReservationId,
                EventTime = log.EventTime
            };
        }

        // Código válido — registrar evento exitoso
        var successLog = AccessLog.CreateSuccess(
            accessCode.Id,
            accessCode.ReservationId,
            dto.EventType);

        await _accessLogRepository.AddAsync(successLog);

        return new ValidateAccessCodeResultDto
        {
            Success = true,
            ReservationId = accessCode.ReservationId,
            EventTime = successLog.EventTime
        };
    }
}