using FluentAssertions;
using Moq;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.Common.Services;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.Reservations;
using StayWize.Domain.Entities;
using StayWize.Services.ExceptionHandling;
using StayWize.Services.Logging;

namespace StayWize.UnitTests.Application.Reservations;

public class CreateReservationCommandHandlerTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock = new();
    private readonly Mock<IPropertyRepository> _propertyRepoMock = new();
    private readonly Mock<IClientRepository> _clientRepoMock = new();
    private readonly Mock<ILogService> _logServiceMock = new();
    private readonly ReservationConcurrencyService _concurrencyService = new();

    private CreateReservationCommandHandler CreateHandler() => new(
        _reservationRepoMock.Object,
        _propertyRepoMock.Object,
        _clientRepoMock.Object,
        _concurrencyService,
        _logServiceMock.Object);

    [Fact]
    public async Task Handle_ValidData_ShouldCreateReservation()
    {
        var propertyId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        _propertyRepoMock.Setup(r => r.ExistsAsync(propertyId)).ReturnsAsync(true);
        _clientRepoMock.Setup(r => r.ExistsAsync(clientId)).ReturnsAsync(true);
        _reservationRepoMock.Setup(r => r.HasOverlapAsync(
            propertyId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(false);

        var dto = new CreateReservationDto
        {
            PropertyId = propertyId,
            ClientId = clientId,
            CheckIn = DateTime.UtcNow.AddDays(1),
            CheckOut = DateTime.UtcNow.AddDays(5),
            GuestCount = 2
        };

        var result = await CreateHandler().Handle(
            new CreateReservationCommand(dto), CancellationToken.None);

        result.Should().NotBeNull();
        result.PropertyId.Should().Be(propertyId);
        result.ClientId.Should().Be(clientId);
    }

    [Fact]
    public async Task Handle_PropertyNotFound_ShouldThrowNotFoundException()
    {
        var propertyId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        _propertyRepoMock.Setup(r => r.ExistsAsync(propertyId)).ReturnsAsync(false);

        var dto = new CreateReservationDto
        {
            PropertyId = propertyId,
            ClientId = clientId,
            CheckIn = DateTime.UtcNow.AddDays(1),
            CheckOut = DateTime.UtcNow.AddDays(5),
            GuestCount = 2
        };

        var action = async () => await CreateHandler().Handle(
            new CreateReservationCommand(dto), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ClientNotFound_ShouldThrowNotFoundException()
    {
        var propertyId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        _propertyRepoMock.Setup(r => r.ExistsAsync(propertyId)).ReturnsAsync(true);
        _clientRepoMock.Setup(r => r.ExistsAsync(clientId)).ReturnsAsync(false);

        var dto = new CreateReservationDto
        {
            PropertyId = propertyId,
            ClientId = clientId,
            CheckIn = DateTime.UtcNow.AddDays(1),
            CheckOut = DateTime.UtcNow.AddDays(5),
            GuestCount = 2
        };

        var action = async () => await CreateHandler().Handle(
            new CreateReservationCommand(dto), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_DateOverlap_ShouldThrowConflictException()
    {
        var propertyId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        _propertyRepoMock.Setup(r => r.ExistsAsync(propertyId)).ReturnsAsync(true);
        _clientRepoMock.Setup(r => r.ExistsAsync(clientId)).ReturnsAsync(true);
        _reservationRepoMock.Setup(r => r.HasOverlapAsync(
            propertyId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(true);

        var dto = new CreateReservationDto
        {
            PropertyId = propertyId,
            ClientId = clientId,
            CheckIn = DateTime.UtcNow.AddDays(1),
            CheckOut = DateTime.UtcNow.AddDays(5),
            GuestCount = 2
        };

        var action = async () => await CreateHandler().Handle(
            new CreateReservationCommand(dto), CancellationToken.None);

        await action.Should().ThrowAsync<ConflictException>();
    }
}