using FluentAssertions;
using StayWize.Domain.Entities;

namespace StayWize.UnitTests.Domain;

public class PropertyTests
{
    [Fact]
    public void Create_WithDefaultValues_ShouldHaveSelfCheckInFalse()
    {
        var property = Property.Create("Casa Test", "Calle 123", "Buenos Aires", "Argentina", 4, Guid.NewGuid());

        property.IsSelfCheckIn.Should().BeFalse();
    }

    [Fact]
    public void Create_WithSelfCheckInTrue_ShouldSetFlag()
    {
        var property = Property.Create("Casa Test", "Calle 123", "Buenos Aires", "Argentina", 4, Guid.NewGuid(),
            isSelfCheckIn: true);

        property.IsSelfCheckIn.Should().BeTrue();
    }

    [Fact]
    public void SetSelfCheckIn_ShouldUpdateFlag()
    {
        var property = Property.Create("Casa Test", "Calle 123", "Buenos Aires", "Argentina", 4, Guid.NewGuid());

        property.SetSelfCheckIn(true);

        property.IsSelfCheckIn.Should().BeTrue();
    }

    [Fact]
    public void Update_ShouldChangeIsSelfCheckIn()
    {
        var property = Property.Create("Casa Test", "Calle 123", "Buenos Aires", "Argentina", 4, Guid.NewGuid(),
            isSelfCheckIn: false);

        property.Update("Casa Nueva", "Calle 456", "Córdoba", "Argentina", 6, isSelfCheckIn: true);

        property.IsSelfCheckIn.Should().BeTrue();
        property.Name.Should().Be("Casa Nueva");
    }
}