namespace StayWize.Services.ExceptionHandling;

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, 409) { }
}