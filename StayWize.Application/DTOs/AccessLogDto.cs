using StayWize.Domain.Enums;

namespace StayWize.Application.DTOs;

public class AccessLogDto
{
    public Guid Id { get; set; }
    public Guid AccessCodeId { get; set; }
    public Guid ReservationId { get; set; }
    public AccessEventType EventType { get; set; }
    public DateTime EventTime { get; set; }
    public bool Success { get; set; }
    public string? FailureReason { get; set; }
}