using StayWize.Domain.Common;
using StayWize.Domain.Enums;

namespace StayWize.Domain.Entities;

public class AccessLog : BaseEntity
{
    public Guid AccessCodeId { get; private set; }
    public Guid ReservationId { get; private set; }
    public AccessEventType EventType { get; private set; }
    public DateTime EventTime { get; private set; }
    public bool Success { get; private set; }
    public string? FailureReason { get; private set; }

    public AccessCode? AccessCode { get; private set; }

    private AccessLog() { }

    public static AccessLog CreateSuccess(Guid accessCodeId,
                                           Guid reservationId,
                                           AccessEventType eventType)
    {
        return new AccessLog
        {
            AccessCodeId = accessCodeId,
            ReservationId = reservationId,
            EventType = eventType,
            EventTime = DateTime.UtcNow,
            Success = true
        };
    }

    public static AccessLog CreateFailure(Guid accessCodeId,
                                           Guid reservationId,
                                           AccessEventType eventType,
                                           string reason)
    {
        return new AccessLog
        {
            AccessCodeId = accessCodeId,
            ReservationId = reservationId,
            EventType = eventType,
            EventTime = DateTime.UtcNow,
            Success = false,
            FailureReason = reason
        };
    }
}