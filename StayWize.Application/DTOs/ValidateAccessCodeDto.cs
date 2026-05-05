using StayWize.Domain.Enums;

namespace StayWize.Application.DTOs;

public class ValidateAccessCodeDto
{
    public string Code { get; set; } = string.Empty;
    public AccessEventType EventType { get; set; }
}