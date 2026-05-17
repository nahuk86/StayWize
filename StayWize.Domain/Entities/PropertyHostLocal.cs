namespace StayWize.Domain.Entities;

public class PropertyHostLocal
{
    public Guid PropertyId { get; private set; }
    public Guid HostLocalId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    public Property? Property { get; private set; }
    public HostLocal? HostLocal { get; private set; }

    private PropertyHostLocal() { }

    public static PropertyHostLocal Create(Guid propertyId, Guid hostLocalId)
    {
        return new PropertyHostLocal
        {
            PropertyId = propertyId,
            HostLocalId = hostLocalId,
            AssignedAt = DateTime.UtcNow
        };
    }
}