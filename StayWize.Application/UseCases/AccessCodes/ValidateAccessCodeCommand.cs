using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Domain.Entities;
using StayWize.Services.Encryption;
using StayWize.Services.Logging;

namespace StayWize.Application.UseCases.AccessCodes;

public record ValidateAccessCodeCommand(ValidateAccessCodeDto Dto)
    : IRequest<ValidateAccessCodeResultDto>;

public class ValidateAccessCodeCommandHandler
    : IRequestHandler<ValidateAccessCodeCommand, ValidateAccessCodeResultDto>
{
    private readonly IAccessCodeRepository _accessCodeRepository;
    private readonly IAccessLogRepository _accessLogRepository;
    private readonly ILogService _logService;
    private readonly IEncryptionService _encryptionService;


    public ValidateAccessCodeCommandHandler(
        IAccessCodeRepository accessCodeRepository,
        IAccessLogRepository accessLogRepository,
        ILogService logService, IEncryptionService encryptionService)
    {
        _accessCodeRepository = accessCodeRepository;
        _accessLogRepository = accessLogRepository;
        _logService = logService;
        _encryptionService = encryptionService;

    }

    public async Task<ValidateAccessCodeResultDto> Handle(
        ValidateAccessCodeCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Encriptamos el código recibido para buscarlo en la DB
        var encryptedCode = _encryptionService.Encrypt(dto.Code);
        var accessCode = await _accessCodeRepository.GetByCodeAsync(encryptedCode);

        if (accessCode is null)
        {
            _logService.LogWarning("Intento de validación con código inexistente. Code: {Code}", dto.Code);
            return new ValidateAccessCodeResultDto
            {
                Success = false,
                FailureReason = "Código de acceso no encontrado."
            };
        }

        if (!accessCode.IsValid())
        {
            _logService.LogWarning("Código de acceso inválido. Code: {Code} Estado: {Status}", dto.Code, accessCode.Status);

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

        var successLog = AccessLog.CreateSuccess(
            accessCode.Id,
            accessCode.ReservationId,
            dto.EventType);

        await _accessLogRepository.AddAsync(successLog);

        _logService.LogInformation("Acceso validado exitosamente. Code: {Code} ReservationId: {ReservationId} EventType: {EventType}",
            dto.Code, accessCode.ReservationId, dto.EventType);

        return new ValidateAccessCodeResultDto
        {
            Success = true,
            ReservationId = accessCode.ReservationId,
            EventTime = successLog.EventTime
        };
    }
}