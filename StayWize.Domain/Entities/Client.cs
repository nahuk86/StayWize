using StayWize.Domain.Common;

namespace StayWize.Domain.Entities;

public class Client : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string DocumentNumber { get; private set; } = string.Empty;

    public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();
    private readonly List<Reservation> _reservations = new();

    private Client() { }

    public static Client Create(string firstName, string lastName,
                                 string email, string phone, string documentNumber)
    {
        return new Client
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            DocumentNumber = documentNumber
        };
    }

    public void Update(string firstName, string lastName,
                       string email, string phone)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
    }
}