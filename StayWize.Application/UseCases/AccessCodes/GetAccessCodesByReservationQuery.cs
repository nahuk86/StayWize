using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Services.Encryption;

namespace StayWize.Application.UseCases.AccessCodes;

public record GetAccessCodesByReservationQuery(Guid ReservationId)
    : IRequest<IEnumerable<AccessCodeDto>>;

public class GetAccessCodesByReservationQueryHandler
    : IRequestHandler<GetAccessCodesByReservationQuery, IEnumerable<AccessCodeDto>>
{
    private readonly IAccessCodeRepository _repository;
    private readonly IEncryptionService _encryptionService;

    public GetAccessCodesByReservationQueryHandler(
        IAccessCodeRepository repository,
        IEncryptionService encryptionService)
    {
        _repository = repository;
        _encryptionService = encryptionService;
    }

    public async Task<IEnumerable<AccessCodeDto>> Handle(
        GetAccessCodesByReservationQuery request,
        CancellationToken cancellationToken)
    {
        var codes = await _repository.GetByReservationIdAsync(request.ReservationId);

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