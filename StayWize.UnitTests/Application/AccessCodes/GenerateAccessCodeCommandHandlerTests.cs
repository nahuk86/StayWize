using FluentAssertions;
using Moq;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.AccessCodes;
using StayWize.Domain.Entities;
using StayWize.Domain.Enums;
using StayWize.Services.Encryption;
using StayWize.Services.ExceptionHandling;
using StayWize.Services.Notifications;

namespace StayWize.UnitTests.Application.AccessCodes;

public class GenerateAccessCodeCommandHandlerTests
{
    private readonly Mock<IAccessCodeRepository> _accessCodeRepoMock = new();
    private readonly Mock<IReservationRepository> _reservationRepoMock = new();
    private readonly Mock<IClientRepository> _clientRepoMock = new();
    private readonly Mock<IEncryptionService> _encryptionServiceMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();

    private GenerateAccessCodeCommandHandler CreateHandler() => new(
        _accessCodeRepoMock.Object,
        _reservationRepoMock.Object,
        _clientRepoMock.Object,
        _encryptionServiceMock.Object,
        _emailServiceMock.Object);

    [Fact]
    public async Task Handle_ConfirmedReservation_ShouldGenerateCode()
    {
        var reservation = Reservation.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(5), 2);
        reservation.Confirm();

        var client = Client.Create("Juan", "Pérez", "juan@test.com", "123", "12345");

        _reservationRepoMock.Setup(r => r.GetByIdAsync(reservation.Id))
            .ReturnsAsync(reservation);
        _clientRepoMock.Setup(r => r.GetByIdAsync(reservation.ClientId))
            .ReturnsAsync(client);
        _encryptionServiceMock.Setup(e => e.Encrypt(It.IsAny<string>()))
            .Returns("encrypted-code");
        _encryptionServiceMock.Setup(e => e.Decrypt(It.IsAny<string>()))
            .Returns("123456");

        var dto = new GenerateAccessCodeDto
        {
            ReservationId = reservation.Id,
            ValidFrom = DateTime.UtcNow.AddDays(1),
            ValidTo = DateTime.UtcNow.AddDays(5),
            Type = AccessCodeType.CheckIn
        };

        var result = await CreateHandler().Handle(
            new GenerateAccessCodeCommand(dto), CancellationToken.None);

        result.Should().NotBeNull();
        result.ReservationId.Should().Be(reservation.Id);
    }

    [Fact]
    public async Task Handle_ReservationNotFound_ShouldThrowNotFoundException()
    {
        _reservationRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Reservation?)null);

        var dto = new GenerateAccessCodeDto
        {
            ReservationId = Guid.NewGuid(),
            ValidFrom = DateTime.UtcNow.AddDays(1),
            ValidTo = DateTime.UtcNow.AddDays(5),
            Type = AccessCodeType.CheckIn
        };

        var action = async () => await CreateHandler().Handle(
            new GenerateAccessCodeCommand(dto), CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ReservationNotConfirmed_ShouldThrowConflictException()
    {
        var reservation = Reservation.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(5), 2);
        // Status es Pending, no Confirmed

        _reservationRepoMock.Setup(r => r.GetByIdAsync(reservation.Id))
            .ReturnsAsync(reservation);

        var dto = new GenerateAccessCodeDto
        {
            ReservationId = reservation.Id,
            ValidFrom = DateTime.UtcNow.AddDays(1),
            ValidTo = DateTime.UtcNow.AddDays(5),
            Type = AccessCodeType.CheckIn
        };

        var action = async () => await CreateHandler().Handle(
            new GenerateAccessCodeCommand(dto), CancellationToken.None);

        await action.Should().ThrowAsync<ConflictException>();
    }
}