namespace StayWize.Application.DTOs;

public class CreateReservationDto
{
    public Guid PropertyId { get; set; }
    public Guid ClientId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int GuestCount { get; set; }
    public string? Notes { get; set; }
}