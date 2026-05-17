namespace StayWize.Application.DTOs;

public class CreatePropertyDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int MaxGuests { get; set; }
    public Guid OwnerId { get; set; }
    public bool IsSelfCheckIn { get; set; } = false;
}