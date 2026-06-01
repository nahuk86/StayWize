namespace StayWize.Domain.Entities;

public enum RegistrationRequestStatus
{
    Pending,
    Approved,
    Rejected
}

public class ClientRegistrationRequest
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string DocumentNumber { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public RegistrationRequestStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ClientRegistrationRequest() { }

    public static ClientRegistrationRequest Create(
        string firstName,
        string lastName,
        string email,
        string documentNumber,
        string phone)
    {
        return new ClientRegistrationRequest
        {
            Id             = Guid.NewGuid(),
            FirstName      = firstName,
            LastName       = lastName,
            Email          = email,
            DocumentNumber = documentNumber,
            Phone          = phone,
            Status         = RegistrationRequestStatus.Pending,
            CreatedAt      = DateTime.UtcNow,
            UpdatedAt      = DateTime.UtcNow
        };
    }

    public void Approve()
    {
        if (Status != RegistrationRequestStatus.Pending)
            throw new InvalidOperationException("Solo se pueden aprobar solicitudes en estado Pending.");

        Status    = RegistrationRequestStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        if (Status != RegistrationRequestStatus.Pending)
            throw new InvalidOperationException("Solo se pueden rechazar solicitudes en estado Pending.");

        Status    = RegistrationRequestStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}