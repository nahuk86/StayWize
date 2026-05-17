using StayWize.Domain.Common;

namespace StayWize.Domain.Entities;

public class HostLocal : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Zone { get; private set; } = string.Empty;
    public bool IsAvailable { get; private set; } = true;

    // Vínculo con Identity
    public string? UserId { get; private set; }

    // Propiedades asignadas
    public IReadOnlyCollection<PropertyHostLocal> PropertyAssignments => _propertyAssignments.AsReadOnly();
    private readonly List<PropertyHostLocal> _propertyAssignments = new();

    private HostLocal() { }

    public static HostLocal Create(string firstName, string lastName,
                                    string email, string phone, string zone,
                                    string? userId = null)
    {
        return new HostLocal
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Zone = zone,
            UserId = userId
        };
    }

    public void Update(string firstName, string lastName,
                       string email, string phone, string zone)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Zone = zone;
    }

    public void SetUserId(string userId) => UserId = userId;
    public void SetAvailability(bool available) => IsAvailable = available;
}