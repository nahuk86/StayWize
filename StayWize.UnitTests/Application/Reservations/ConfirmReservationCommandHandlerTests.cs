using FluentAssertions;
using Moq;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.UseCases.Reservations;
using StayWize.Domain.Entities;
using StayWize.Domain.Enums;
using StayWize.Services.Notifications;

namespace StayWize.UnitTests.Application.Reservations;

public class ConfirmReservationCommandHandlerTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock = new();
    private readonly Mock<IClientRepository> _clientRepoMock = new();
    private readonly Mock<IPropertyRepository> _propertyRepoMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();

    private ConfirmReservationCommandHandler CreateHandler() => new(
        _reservationRepoMock.Object,
        _clientRepoMock.Object,
        _propertyRepoMock.Object,
        _emailServiceMock.Object);

    [Fact]
    public async Task Handle_ExistingReservation_ShouldConfirmAndSendEmail()
    {
        var reservation = Reservation.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(5), 2);

        var client = Client.Create("Juan", "Pérez", "juan@test.com", "123", "12345");
        var property = Property.Create("Depto", "Av. Test 123", "CABA", "Argentina", 4, Guid.NewGuid());

        _reservationRepoMock.Setup(r => r.GetByIdAsync(reservation.Id))
            .ReturnsAsync(reservation);
        _clientRepoMock.Setup(r => r.GetByIdAsync(reservation.ClientId))
            .ReturnsAsync(client);
        _propertyRepoMock.Setup(r => r.GetByIdAsync(reservation.PropertyId))
            .ReturnsAsync(property);

        var result = await CreateHandler().Handle(
            new ConfirmReservationCommand(reservation.Id), CancellationToken.None);

        result.Should().BeTrue();
        reservation.Status.Should().Be(ReservationStatus.Confirmed);
    }

    [Fact]
    public async Task Handle_ReservationNotFound_ShouldReturnFalse()
    {
        _reservationRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Reservation?)null);

        var result = await CreateHandler().Handle(
            new ConfirmReservationCommand(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeFalse();
    }
}