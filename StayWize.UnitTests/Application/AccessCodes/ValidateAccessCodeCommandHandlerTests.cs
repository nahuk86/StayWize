using FluentAssertions;
using Moq;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.AccessCodes;
using StayWize.Domain.Entities;
using StayWize.Domain.Enums;
using StayWize.Services.Encryption;
using StayWize.Services.Logging;

namespace StayWize.UnitTests.Application.AccessCodes;

public class ValidateAccessCodeCommandHandlerTests
{
    private readonly Mock<IAccessCodeRepository> _accessCodeRepoMock = new();
    private readonly Mock<IAccessLogRepository> _accessLogRepoMock = new();
    private readonly Mock<ILogService> _logServiceMock = new();
    private readonly Mock<IEncryptionService> _encryptionServiceMock = new();

    private ValidateAccessCodeCommandHandler CreateHandler() => new(
        _accessCodeRepoMock.Object,
        _accessLogRepoMock.Object,
        _logServiceMock.Object,
        _encryptionServiceMock.Object);

    [Fact]
    public async Task Handle_ValidCode_ShouldReturnSuccess()
    {
        var accessCode = AccessCode.Create(
            Guid.NewGuid(),
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddHours(1),
            AccessCodeType.CheckIn);

        _encryptionServiceMock.Setup(e => e.Encrypt(It.IsAny<string>()))
            .Returns("encrypted-code");
        _accessCodeRepoMock.Setup(r => r.GetByCodeAsync("encrypted-code"))
            .ReturnsAsync(accessCode);

        var dto = new ValidateAccessCodeDto
        {
            Code = accessCode.Code,
            EventType = AccessEventType.Entry
        };

        var result = await CreateHandler().Handle(
            new ValidateAccessCodeCommand(dto), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.ReservationId.Should().Be(accessCode.ReservationId);
    }

    [Fact]
    public async Task Handle_CodeNotFound_ShouldReturnFailure()
    {
        _encryptionServiceMock.Setup(e => e.Encrypt(It.IsAny<string>()))
            .Returns("encrypted-code");
        _accessCodeRepoMock.Setup(r => r.GetByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((AccessCode?)null);

        var dto = new ValidateAccessCodeDto
        {
            Code = "999999",
            EventType = AccessEventType.Entry
        };

        var result = await CreateHandler().Handle(
            new ValidateAccessCodeCommand(dto), CancellationToken.None);

        result.Success.Should().BeFalse();
        result.FailureReason.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_RevokedCode_ShouldReturnFailure()
    {
        var accessCode = AccessCode.Create(
            Guid.NewGuid(),
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddHours(1),
            AccessCodeType.CheckIn);
        accessCode.Revoke();

        _encryptionServiceMock.Setup(e => e.Encrypt(It.IsAny<string>()))
            .Returns("encrypted-code");
        _accessCodeRepoMock.Setup(r => r.GetByCodeAsync("encrypted-code"))
            .ReturnsAsync(accessCode);

        var dto = new ValidateAccessCodeDto
        {
            Code = accessCode.Code,
            EventType = AccessEventType.Entry
        };

        var result = await CreateHandler().Handle(
            new ValidateAccessCodeCommand(dto), CancellationToken.None);

        result.Success.Should().BeFalse();
    }
}