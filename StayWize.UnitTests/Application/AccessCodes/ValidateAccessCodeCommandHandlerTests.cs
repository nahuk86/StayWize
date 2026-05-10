using FluentAssertions;
using Moq;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.AccessCodes;
using StayWize.Domain.Entities;
using StayWize.Domain.Enums;
using StayWize.Services.Logging;

namespace StayWize.UnitTests.Application.AccessCodes;

public class ValidateAccessCodeCommandHandlerTests
{
    private readonly Mock<IAccessCodeRepository> _accessCodeRepoMock = new();
    private readonly Mock<IAccessLogRepository> _accessLogRepoMock = new();
    private readonly Mock<ILogService> _logServiceMock = new();

    private ValidateAccessCodeCommandHandler CreateHandler() => new(
        _accessCodeRepoMock.Object,
        _accessLogRepoMock.Object,
        _logServiceMock.Object);

    [Fact]
    public async Task Handle_ValidCode_ShouldReturnSuccess()
    {
        var accessCode = AccessCode.Create(
            Guid.NewGuid(),
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddHours(1),
            AccessCodeType.CheckIn);

        _accessCodeRepoMock.Setup(r => r.GetByPlainCodeAsync(It.IsAny<string>()))
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
        _accessCodeRepoMock.Setup(r => r.GetByPlainCodeAsync(It.IsAny<string>()))
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

        _accessCodeRepoMock.Setup(r => r.GetByPlainCodeAsync(It.IsAny<string>()))
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