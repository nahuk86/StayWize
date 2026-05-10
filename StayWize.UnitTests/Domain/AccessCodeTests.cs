using FluentAssertions;
using StayWize.Domain.Entities;
using StayWize.Domain.Enums;

namespace StayWize.UnitTests.Domain;

public class AccessCodeTests
{
    private readonly Guid _reservationId = Guid.NewGuid();
    private readonly DateTime _validFrom = DateTime.UtcNow.AddHours(-1);
    private readonly DateTime _validTo = DateTime.UtcNow.AddHours(1);

    [Fact]
    public void Create_ValidData_ShouldCreateAccessCode()
    {
        var code = AccessCode.Create(_reservationId, _validFrom, _validTo, AccessCodeType.CheckIn);

        code.Should().NotBeNull();
        code.ReservationId.Should().Be(_reservationId);
        code.Status.Should().Be(AccessCodeStatus.Active);
        code.Code.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Create_ValidToBeforeValidFrom_ShouldThrowArgumentException()
    {
        var action = () => AccessCode.Create(
            _reservationId, _validTo, _validFrom, AccessCodeType.CheckIn);

        action.Should().Throw<ArgumentException>()
            .WithMessage("*ValidTo*");
    }

    [Fact]
    public void IsValid_ActiveAndWithinRange_ShouldReturnTrue()
    {
        var code = AccessCode.Create(_reservationId, _validFrom, _validTo, AccessCodeType.CheckIn);

        code.IsValid().Should().BeTrue();
    }

    [Fact]
    public void IsValid_Revoked_ShouldReturnFalse()
    {
        var code = AccessCode.Create(_reservationId, _validFrom, _validTo, AccessCodeType.CheckIn);
        code.Revoke();

        code.IsValid().Should().BeFalse();
    }

    [Fact]
    public void IsValid_Expired_ShouldReturnFalse()
    {
        var pastFrom = DateTime.UtcNow.AddHours(-2);
        var pastTo = DateTime.UtcNow.AddHours(-1);
        var code = AccessCode.Create(_reservationId, pastFrom, pastTo, AccessCodeType.CheckIn);

        code.IsValid().Should().BeFalse();
    }

    [Fact]
    public void Revoke_ShouldChangeStatusToRevoked()
    {
        var code = AccessCode.Create(_reservationId, _validFrom, _validTo, AccessCodeType.CheckIn);

        code.Revoke();

        code.Status.Should().Be(AccessCodeStatus.Revoked);
    }

    [Fact]
    public void MarkAsExpired_ShouldChangeStatusToExpired()
    {
        var code = AccessCode.Create(_reservationId, _validFrom, _validTo, AccessCodeType.CheckIn);

        code.MarkAsExpired();

        code.Status.Should().Be(AccessCodeStatus.Expired);
    }
}