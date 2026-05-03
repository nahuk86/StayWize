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

    // Relaciones
    public Guid OwnerId { get; private set; }
    public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();
    private readonly List<Reservation> _reservations = new();

    private Property() { }

    public static Property Create(string name, string address, string city,
                                   string country, int maxGuests, Guid ownerId)
    {
        return new Property
        {
            Name = name,
            Address = address,
            City = city,
            Country = country,
            MaxGuests = maxGuests,
            OwnerId = ownerId
        };
    }

    public void Update(string name, string address, string city,
                       string country, int maxGuests)
    {
        Name = name;
        Address = address;
        City = city;
        Country = country;
        MaxGuests = maxGuests;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}