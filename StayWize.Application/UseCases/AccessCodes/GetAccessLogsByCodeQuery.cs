using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.AccessCodes;

public record GetAccessLogsByCodeQuery(Guid AccessCodeId)
    : IRequest<IEnumerable<AccessLogDto>>;

public class GetAccessLogsByCodeQueryHandler
    : IRequestHandler<GetAccessLogsByCodeQuery, IEnumerable<AccessLogDto>>
{
    private readonly IAccessLogRepository _repository;

    public GetAccessLogsByCodeQueryHandler(IAccessLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AccessLogDto>> Handle(
        GetAccessLogsByCodeQuery request,
        CancellationToken cancellationToken)
    {
        var logs = await _repository.GetByAccessCodeIdAsync(request.AccessCodeId);

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