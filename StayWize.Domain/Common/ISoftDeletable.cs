namespace StayWize.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
}