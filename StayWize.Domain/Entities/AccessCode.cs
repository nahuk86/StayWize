using StayWize.Domain.Common;
using StayWize.Domain.Enums;

namespace StayWize.Domain.Entities;

public class AccessCode : BaseEntity
{
    public Guid ReservationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidTo { get; private set; }
    public AccessCodeStatus Status { get; private set; } = AccessCodeStatus.Active;
    public AccessCodeType Type { get; private set; }

    public Reservation? Reservation { get; private set; }
    public IReadOnlyCollection<AccessLog> AccessLogs => _accessLogs.AsReadOnly();
    private readonly List<AccessLog> _accessLogs = new();

    private AccessCode() { }

    public static AccessCode Create(Guid reservationId, DateTime validFrom,
                                     DateTime validTo, AccessCodeType type)
    {
        if (validTo <= validFrom)
            throw new ArgumentException("ValidTo debe ser posterior a ValidFrom.");

        return new AccessCode
        {
            ReservationId = reservationId,
            Code = GenerateCode(),
            ValidFrom = validFrom,
            ValidTo = validTo,
            Type = type
        };
    }

    public bool IsValid() =>
        Status == AccessCodeStatus.Active &&
        DateTime.UtcNow >= ValidFrom &&
        DateTime.UtcNow <= ValidTo;

    public void Revoke() => Status = AccessCodeStatus.Revoked;
    public void MarkAsExpired() => Status = AccessCodeStatus.Expired;

    private static string GenerateCode() =>
        new Random().Next(100000, 999999).ToString();
}