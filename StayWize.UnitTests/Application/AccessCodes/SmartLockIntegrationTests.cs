using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.AccessCodes;
using StayWize.Domain.Entities;
using StayWize.Domain.Enums;
using StayWize.Infrastructure.Services;
using StayWize.Services.Authentication;
using StayWize.Services.Encryption;
using StayWize.Services.Notifications;

namespace StayWize.UnitTests.Application.AccessCodes;

public class SmartLockIntegrationTests
{
    private readonly Mock<IAccessCodeRepository> _accessCodeRepoMock = new();
    private readonly Mock<IReservationRepository> _reservationRepoMock = new();
    private readonly Mock<IPropertyRepository> _propertyRepoMock = new();
    private readonly Mock<IClientRepository> _clientRepoMock = new();
    private readonly Mock<IEncryptionService> _encryptionServiceMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<UserManager<AppUser>> _userManagerMock = CreateUserManagerMock();
    private readonly Mock<ISmartLockService> _smartLockServiceMock = new();

    private static Mock<UserManager<AppUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<AppUser>>();
        return new Mock<UserManager<AppUser>>(
            store.Object, null, null, null, null, null, null, null, null);
    }

    private GenerateAccessCodeCommandHandler CreateHandler() => new(
        _accessCodeRepoMock.Object,
        _reservationRepoMock.Object,
        _propertyRepoMock.Object,
        _clientRepoMock.Object,
        _encryptionServiceMock.Object,
        _emailServiceMock.Object,
        _userManagerMock.Object,
        _smartLockServiceMock.Object);

    [Fact]
    public async Task Handle_PropertyWithLockDevice_ShouldCallSetCodeAsync()
    {
        var lockDeviceId = "lock-device-abc123";
        var property = Property.Create("Casa", "Calle 1", "BA", "AR", 4, Guid.NewGuid(),
            isSelfCheckIn: true, lockDeviceId: lockDeviceId);

        var reservation = Reservation.Create(
            property.Id, Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(5), 2);
        reservation.Confirm();

        _reservationRepoMock.Setup(r => r.GetByIdAsync(reservation.Id)).ReturnsAsync(reservation);
        _propertyRepoMock.Setup(r => r.GetByIdAsync(property.Id)).ReturnsAsync(property);
        _clientRepoMock.Setup(r => r.GetByIdAsync(reservation.ClientId)).ReturnsAsync((StayWize.Domain.Entities.Client?)null);
        _userManagerMock.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((AppUser?)null);
        _encryptionServiceMock.Setup(e => e.Encrypt(It.IsAny<string>())).Returns("encrypted");
        _smartLockServiceMock
            .Setup(s => s.SetCodeAsync(lockDeviceId, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(SmartLockResult.Ok());

        var dto = new GenerateAccessCodeDto
        {
            ReservationId = reservation.Id,
            ValidFrom = DateTime.UtcNow.AddDays(1),
            ValidTo = DateTime.UtcNow.AddDays(5),
            Type = AccessCodeType.CheckIn
        };

        await CreateHandler().Handle(new GenerateAccessCodeCommand(dto), CancellationToken.None);

        // Dar tiempo al fire-and-forget para ejecutarse
        await Task.Delay(50);

        _smartLockServiceMock.Verify(
            s => s.SetCodeAsync(lockDeviceId, It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_PropertyWithoutLockDevice_ShouldNotCallSetCodeAsync()
    {
        var property = Property.Create("Casa", "Calle 1", "BA", "AR", 4, Guid.NewGuid(),
            isSelfCheckIn: true, lockDeviceId: null);

        var reservation = Reservation.Create(
            property.Id, Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(5), 2);
        reservation.Confirm();

        _reservationRepoMock.Setup(r => r.GetByIdAsync(reservation.Id)).ReturnsAsync(reservation);
        _propertyRepoMock.Setup(r => r.GetByIdAsync(property.Id)).ReturnsAsync(property);
        _clientRepoMock.Setup(r => r.GetByIdAsync(reservation.ClientId)).ReturnsAsync((StayWize.Domain.Entities.Client?)null);
        _userManagerMock.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((AppUser?)null);
        _encryptionServiceMock.Setup(e => e.Encrypt(It.IsAny<string>())).Returns("encrypted");

        var dto = new GenerateAccessCodeDto
        {
            ReservationId = reservation.Id,
            ValidFrom = DateTime.UtcNow.AddDays(1),
            ValidTo = DateTime.UtcNow.AddDays(5),
            Type = AccessCodeType.CheckIn
        };

        await CreateHandler().Handle(new GenerateAccessCodeCommand(dto), CancellationToken.None);

        _smartLockServiceMock.Verify(
            s => s.SetCodeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()),
            Times.Never);
    }
}

public class StubSmartLockServiceTests
{
    [Fact]
    public async Task SetCodeAsync_ShouldReturnSuccess()
    {
        var logger = new Mock<ILogger<StubSmartLockService>>();
        var service = new StubSmartLockService(logger.Object);

        var result = await service.SetCodeAsync("device-1", "123456",
            DateTime.UtcNow, DateTime.UtcNow.AddDays(5));

        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task RevokeCodeAsync_ShouldReturnSuccess()
    {
        var logger = new Mock<ILogger<StubSmartLockService>>();
        var service = new StubSmartLockService(logger.Object);

        var result = await service.RevokeCodeAsync("device-1", "123456");

        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }
}