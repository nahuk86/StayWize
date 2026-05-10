using FluentAssertions;
using Moq;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.UseCases.Reservations;
using StayWize.Domain.Entities;
using StayWize.Domain.Enums;

namespace StayWize.UnitTests.Application.Reservations;

public class CancelReservationCommandHandlerTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock = new();

    [Fact]
    public async Task Handle_ExistingReservation_ShouldCancelReservation()
    {
        var reservation = Reservation.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(5), 2);

        _reservationRepoMock.Setup(r => r.GetByIdAsync(reservation.Id))
            .ReturnsAsync(reservation);

        var handler = new CancelReservationCommandHandler(_reservationRepoMock.Object);
        var result = await handler.Handle(
            new CancelReservationCommand(reservation.Id), CancellationToken.None);

        result.Should().BeTrue();
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public async Task Handle_ReservationNotFound_ShouldReturnFalse()
    {
        _reservationRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Reservation?)null);

        var handler = new CancelReservationCommandHandler(_reservationRepoMock.Object);
        var result = await handler.Handle(
            new CancelReservationCommand(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeFalse();
    }
}