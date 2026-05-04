using UUIDNext;

namespace StayWize.Domain.Common;

public abstract class BaseEntity : ISoftDeletable
{
    public Guid Id { get; private set; } = Uuid.NewDatabaseFriendly(Database.SqlServer);

    // Auditoría
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Soft delete
    public bool IsDeleted { get; private set; } = false;
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    public void SetCreatedBy(string username)
    {
        CreatedBy = username;
    }

    public void MarkAsUpdated(string username)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = username;
    }

    public void SoftDelete(string username)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = username;
    }
}