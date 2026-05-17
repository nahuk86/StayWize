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

    private Property MakeSelfCheckInProperty()
        => Property.Create("Casa", "Calle 1", "BA", "AR", 4, Guid.NewGuid(), isSelfCheckIn: true);

    private Property MakeNonSelfCheckInProperty()
        => Property.Create("Casa", "Calle 1", "BA", "AR", 4, Guid.NewGuid(), isSelfCheckIn: false);

    [Fact]
    public async Task Handle_ValidData_ShouldCreateReservation()
    {
        var property = MakeSelfCheckInProperty();
        var clientId = Guid.NewGuid();

        _propertyRepoMock.Setup(r => r.GetByIdAsync(property.Id)).ReturnsAsync(property);
        _clientRepoMock.Setup(r => r.ExistsAsync(clientId)).ReturnsAsync(true);
        _reservationRepoMock.Setup(r => r.HasOverlapAsync(
            property.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(false);

        var dto = new CreateReservationDto
        {
            PropertyId = property.Id,
            ClientId = clientId,
            CheckIn = DateTime.UtcNow.AddDays(1),
            CheckOut = DateTime.UtcNow.AddDays(5),
            GuestCount = 2
        };

        var result = await CreateHandler().Handle(new CreateReservationCommand(dto), CancellationToken.None);

        result.Should().NotBeNull();
        result.PropertyId.Should().Be(property.Id);
        result.ClientId.Should().Be(clientId);
    }

    [Fact]
    public async Task Handle_PropertyNotFound_ShouldThrowNotFoundException()
    {
        _propertyRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Property?)null);

        var dto = new CreateReservationDto
        {
            PropertyId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            CheckIn = DateTime.UtcNow.AddDays(1),
            CheckOut = DateTime.UtcNow.AddDays(5),
            GuestCount = 2
        };

        var action = async () => await CreateHandler().Handle(new CreateReservationCommand(dto), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ClientNotFound_ShouldThrowNotFoundException()
    {
        var property = MakeSelfCheckInProperty();

        _propertyRepoMock.Setup(r => r.GetByIdAsync(property.Id)).ReturnsAsync(property);
        _clientRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        var dto = new CreateReservationDto
        {
            PropertyId = property.Id,
            ClientId = Guid.NewGuid(),
            CheckIn = DateTime.UtcNow.AddDays(1),
            CheckOut = DateTime.UtcNow.AddDays(5),
            GuestCount = 2
        };

        var action = async () => await CreateHandler().Handle(new CreateReservationCommand(dto), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_DateOverlap_ShouldThrowConflictException()
    {
        var property = MakeSelfCheckInProperty();
        var clientId = Guid.NewGuid();

        _propertyRepoMock.Setup(r => r.GetByIdAsync(property.Id)).ReturnsAsync(property);
        _clientRepoMock.Setup(r => r.ExistsAsync(clientId)).ReturnsAsync(true);
        _reservationRepoMock.Setup(r => r.HasOverlapAsync(
            property.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(true);

        var dto = new CreateReservationDto
        {
            PropertyId = property.Id,
            ClientId = clientId,
            CheckIn = DateTime.UtcNow.AddDays(1),
            CheckOut = DateTime.UtcNow.AddDays(5),
            GuestCount = 2
        };

        var action = async () => await CreateHandler().Handle(new CreateReservationCommand(dto), CancellationToken.None);

        await action.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_NonSelfCheckIn_NoHostLocal_ShouldThrowValidationException()
    {
        var property = MakeNonSelfCheckInProperty();

        _propertyRepoMock.Setup(r => r.GetByIdAsync(property.Id)).ReturnsAsync(property);

        var dto = new CreateReservationDto
        {
            PropertyId = property.Id,
            ClientId = Guid.NewGuid(),
            CheckIn = DateTime.UtcNow.AddDays(1),
            CheckOut = DateTime.UtcNow.AddDays(5),
            GuestCount = 2,
            HostLocalId = null
        };

        var action = async () => await CreateHandler().Handle(new CreateReservationCommand(dto), CancellationToken.None);

        await action.Should().ThrowAsync<ValidationException>()
            .WithMessage("*host local*");
    }

    [Fact]
    public async Task Handle_NonSelfCheckIn_WithHostLocal_ShouldCreate()
    {
        var property = MakeNonSelfCheckInProperty();
        var clientId = Guid.NewGuid();
        var hostLocalId = Guid.NewGuid();

        _propertyRepoMock.Setup(r => r.GetByIdAsync(property.Id)).ReturnsAsync(property);
        _clientRepoMock.Setup(r => r.ExistsAsync(clientId)).ReturnsAsync(true);
        _reservationRepoMock.Setup(r => r.HasOverlapAsync(
            property.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(false);

        var dto = new CreateReservationDto
        {
            PropertyId = property.Id,
            ClientId = clientId,
            CheckIn = DateTime.UtcNow.AddDays(1),
            CheckOut = DateTime.UtcNow.AddDays(5),
            GuestCount = 2,
            HostLocalId = hostLocalId
        };

        var result = await CreateHandler().Handle(new CreateReservationCommand(dto), CancellationToken.None);

        result.Should().NotBeNull();
        result.HostLocalId.Should().Be(hostLocalId);
    }
}