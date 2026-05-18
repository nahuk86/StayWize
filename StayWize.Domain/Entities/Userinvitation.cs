using StayWize.Domain.Common;

namespace StayWize.Domain.Entities;

public class UserInvitation : BaseEntity
{
    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public bool IsUsed => UsedAt.HasValue;
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;

    private UserInvitation() { }

    public static UserInvitation Create(
        string email, string firstName, string lastName,
        string role, string tokenHash, int expirationHours = 48)
    {
        return new UserInvitation
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddHours(expirationHours)
        };
    }

    public void MarkAsUsed()
    {
        UsedAt = DateTime.UtcNow;
        MarkAsUpdated("system");
    }
}