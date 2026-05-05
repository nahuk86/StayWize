using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.AccessCodes;

public record GetAccessCodesByReservationQuery(Guid ReservationId)
    : IRequest<IEnumerable<AccessCodeDto>>;

public class GetAccessCodesByReservationQueryHandler
    : IRequestHandler<GetAccessCodesByReservationQuery, IEnumerable<AccessCodeDto>>
{
    private readonly IAccessCodeRepository _repository;

    public GetAccessCodesByReservationQueryHandler(IAccessCodeRepository repository)
    {
        _repository = repository;
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
            Code = c.Code,
            ValidFrom = c.ValidFrom,
            ValidTo = c.ValidTo,
            Status = c.Status,
            Type = c.Type,
            CreatedAt = c.CreatedAt
        });
    }
}