namespace StayWize.Services.ExceptionHandling;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}