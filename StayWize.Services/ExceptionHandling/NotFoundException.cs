namespace StayWize.Services.ExceptionHandling;

public class NotFoundException : AppException
{
    public string? EntityName { get; }
    public Guid? EntityId { get; }

    public NotFoundException(string message) : base(message, 404) { }

    public NotFoundException(string entityName, Guid entityId)
        : base(string.Empty, 404)
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}