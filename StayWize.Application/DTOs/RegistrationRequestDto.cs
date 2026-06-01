namespace StayWize.Application.DTOs;

public class RegistrationRequestDto
{
    public Guid   Id             { get; set; }
    public string FirstName      { get; set; } = string.Empty;
    public string LastName       { get; set; } = string.Empty;
    public string Email          { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string Phone          { get; set; } = string.Empty;
    public string Status         { get; set; } = string.Empty;
    public DateTime CreatedAt    { get; set; }
}