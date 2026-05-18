using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Services.Encryption;

namespace StayWize.Application.UseCases.AccessCodes;

public record GetAccessCodesByClientQuery(Guid ClientId) : IRequest<IEnumerable<AccessCodeDto>>;

public class GetAccessCodesByClientQueryHandler
    : IRequestHandler<GetAccessCodesByClientQuery, IEnumerable<AccessCodeDto>>
{
    private readonly IAccessCodeRepository _accessCodeRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IEncryptionService _encryptionService;

    public GetAccessCodesByClientQueryHandler(
        IAccessCodeRepository accessCodeRepository,
        IReservationRepository reservationRepository,
        IEncryptionService encryptionService)
    {
        _accessCodeRepository = accessCodeRepository;
        _reservationRepository = reservationRepository;
        _encryptionService = encryptionService;
    }

    public async Task<IEnumerable<AccessCodeDto>> Handle(
        GetAccessCodesByClientQuery request,
        CancellationToken cancellationToken)
    {
        // Obtener todas las reservas del cliente
        var reservations = await _reservationRepository.GetByClientIdAsync(request.ClientId);
        var reservationIds = reservations.Select(r => r.Id).ToList();

        if (!reservationIds.Any())
            return Enumerable.Empty<AccessCodeDto>();

        var codes = await _accessCodeRepository.GetByReservationIdsAsync(reservationIds);

        return codes.Select(c => new AccessCodeDto
        {
            Id = c.Id,
            ReservationId = c.ReservationId,
            Code = TryDecrypt(c.Code),
            ValidFrom = c.ValidFrom,
            ValidTo = c.ValidTo,
            Status = c.Status,
            Type = c.Type,
            CreatedAt = c.CreatedAt
        });
    }

    private string TryDecrypt(string code)
    {
        try { return _encryptionService.Decrypt(code); }
        catch { return code; }
    }
}