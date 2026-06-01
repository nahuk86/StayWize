namespace StayWize.Application.DTOs;

public class CreateRegistrationRequestDto
{
    public string FirstName      { get; set; } = string.Empty;
    public string LastName       { get; set; } = string.Empty;
    public string Email          { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string Phone          { get; set; } = string.Empty;
}