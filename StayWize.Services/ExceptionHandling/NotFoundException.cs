namespace StayWize.Services.ExceptionHandling;

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, 404) { }

    public NotFoundException(string entity, Guid id)
        : base($"{entity} con ID {id} no fue encontrado.", 404) { }
}