using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.AccessCodes;

public record GetAccessLogsByReservationQuery(Guid ReservationId)
    : IRequest<IEnumerable<AccessLogDto>>;

public class GetAccessLogsByReservationQueryHandler
    : IRequestHandler<GetAccessLogsByReservationQuery, IEnumerable<AccessLogDto>>
{
    private readonly IAccessLogRepository _repository;

    public GetAccessLogsByReservationQueryHandler(IAccessLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AccessLogDto>> Handle(
        GetAccessLogsByReservationQuery request,
        CancellationToken cancellationToken)
    {
        var logs = await _repository.GetByReservationIdAsync(request.ReservationId);

        return logs.Select(l => new AccessLogDto
        {
            Id = l.Id,
            AccessCodeId = l.AccessCodeId,
            ReservationId = l.ReservationId,
            EventType = l.EventType,
            EventTime = l.EventTime,
            Success = l.Success,
            FailureReason = l.FailureReason
        });
    }
}