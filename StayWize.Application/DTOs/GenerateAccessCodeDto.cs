using StayWize.Domain.Enums;

namespace StayWize.Application.DTOs;

public class GenerateAccessCodeDto
{
    public Guid ReservationId { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public AccessCodeType Type { get; set; }
}