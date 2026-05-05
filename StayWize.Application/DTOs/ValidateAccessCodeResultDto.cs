namespace StayWize.Application.DTOs;

public class ValidateAccessCodeResultDto
{
    public bool Success { get; set; }
    public string? FailureReason { get; set; }
    public Guid? ReservationId { get; set; }
    public DateTime? EventTime { get; set; }
}