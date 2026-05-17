using StayWize.Domain.Common;

namespace StayWize.Domain.Entities;

public class Property : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public int MaxGuests { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsSelfCheckIn { get; private set; } = false;

    public Guid OwnerId { get; private set; }

    public IReadOnlyCollection<PropertyHostLocal> HostLocalAssignments => _hostLocalAssignments.AsReadOnly();
    private readonly List<PropertyHostLocal> _hostLocalAssignments = new();

    public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();
    private readonly List<Reservation> _reservations = new();

    private Property() { }

    public static Property Create(string name, string address, string city,
                                   string country, int maxGuests, Guid ownerId,
                                   bool isSelfCheckIn = false)
    {
        return new Property
        {
            Name = name,
            Address = address,
            City = city,
            Country = country,
            MaxGuests = maxGuests,
            OwnerId = ownerId,
            IsSelfCheckIn = isSelfCheckIn
        };
    }

    public void Update(string name, string address, string city,
                       string country, int maxGuests, bool isSelfCheckIn)
    {
        Name = name;
        Address = address;
        City = city;
        Country = country;
        MaxGuests = maxGuests;
        IsSelfCheckIn = isSelfCheckIn;
    }

    public void SetSelfCheckIn(bool value) => IsSelfCheckIn = value;

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}