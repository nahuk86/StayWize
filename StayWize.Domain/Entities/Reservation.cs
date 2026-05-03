using StayWize.Domain.Common;
using StayWize.Domain.Enums;

namespace StayWize.Domain.Entities;

public class Reservation : BaseEntity
{
    public Guid PropertyId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid? HostLocalId { get; private set; }
    public DateTime CheckIn { get; private set; }
    public DateTime CheckOut { get; private set; }
    public int GuestCount { get; private set; }
    public ReservationStatus Status { get; private set; } = ReservationStatus.Pending;
    public string? Notes { get; private set; }

    // Navegación
    public Property? Property { get; private set; }
    public Client? Client { get; private set; }
    public HostLocal? HostLocal { get; private set; }

    public IReadOnlyCollection<AccessCode> AccessCodes => _accessCodes.AsReadOnly();
    private readonly List<AccessCode> _accessCodes = new();

    private Reservation() { }

    public static Reservation Create(Guid propertyId, Guid clientId,
                                      DateTime checkIn, DateTime checkOut,
                                      int guestCount, string? notes = null)
    {
        if (checkOut <= checkIn)
            throw new ArgumentException("CheckOut debe ser posterior a CheckIn.");

        if (guestCount <= 0)
            throw new ArgumentException("La cantidad de huéspedes debe ser mayor a cero.");

        return new Reservation
        {
            PropertyId = propertyId,
            ClientId = clientId,
            CheckIn = checkIn,
            CheckOut = checkOut,
            GuestCount = guestCount,
            Notes = notes
        };
    }

    public void AssignHostLocal(Guid hostLocalId) => HostLocalId = hostLocalId;
    public void Confirm() => Status = ReservationStatus.Confirmed;
    public void Cancel() => Status = ReservationStatus.Cancelled;
    public void Complete() => Status = ReservationStatus.Completed;
}