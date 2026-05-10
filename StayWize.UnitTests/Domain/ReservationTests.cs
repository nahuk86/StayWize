using FluentAssertions;
using StayWize.Domain.Entities;
using StayWize.Domain.Enums;

namespace StayWize.UnitTests.Domain;

public class ReservationTests
{
    private readonly Guid _propertyId = Guid.NewGuid();
    private readonly Guid _clientId = Guid.NewGuid();
    private readonly DateTime _checkIn = DateTime.UtcNow.AddDays(1);
    private readonly DateTime _checkOut = DateTime.UtcNow.AddDays(5);

    [Fact]
    public void Create_ValidData_ShouldCreateReservation()
    {
        var reservation = Reservation.Create(_propertyId, _clientId, _checkIn, _checkOut, 2);

        reservation.Should().NotBeNull();
        reservation.PropertyId.Should().Be(_propertyId);
        reservation.ClientId.Should().Be(_clientId);
        reservation.GuestCount.Should().Be(2);
        reservation.Status.Should().Be(ReservationStatus.Pending);
    }

    [Fact]
    public void Create_CheckOutBeforeCheckIn_ShouldThrowArgumentException()
    {
        var action = () => Reservation.Create(
            _propertyId, _clientId,
            _checkOut, _checkIn, 2);

        action.Should().Throw<ArgumentException>()
            .WithMessage("*CheckOut*");
    }

    [Fact]
    public void Create_ZeroGuestCount_ShouldThrowArgumentException()
    {
        var action = () => Reservation.Create(
            _propertyId, _clientId,
            _checkIn, _checkOut, 0);

        action.Should().Throw<ArgumentException>()
            .WithMessage("*huéspedes*");
    }

    [Fact]
    public void Confirm_ShouldChangeStatusToConfirmed()
    {
        var reservation = Reservation.Create(_propertyId, _clientId, _checkIn, _checkOut, 2);

        reservation.Confirm();

        reservation.Status.Should().Be(ReservationStatus.Confirmed);
    }

    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled()
    {
        var reservation = Reservation.Create(_propertyId, _clientId, _checkIn, _checkOut, 2);

        reservation.Cancel();

        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public void AssignHostLocal_ShouldSetHostLocalId()
    {
        var reservation = Reservation.Create(_propertyId, _clientId, _checkIn, _checkOut, 2);
        var hostLocalId = Guid.NewGuid();

        reservation.AssignHostLocal(hostLocalId);

        reservation.HostLocalId.Should().Be(hostLocalId);
    }
}