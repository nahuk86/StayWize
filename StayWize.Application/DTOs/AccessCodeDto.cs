using StayWize.Domain.Enums;

namespace StayWize.Application.DTOs;

public class AccessCodeDto
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public AccessCodeStatus Status { get; set; }
    public AccessCodeType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}