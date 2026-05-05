namespace StayWize.Services.ExceptionHandling;

public class ValidationException : AppException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message, 400)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Se produjeron uno o más errores de validación.", 400)
    {
        Errors = new Dictionary<string, string[]>(errors);
    }
}