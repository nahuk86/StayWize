using FluentAssertions;
using StayWize.Domain.Entities;

namespace StayWize.UnitTests.Domain;

public class BaseEntityTests
{
    [Fact]
    public void SoftDelete_ShouldSetIsDeletedAndMetadata()
    {
        var client = Client.Create("Juan", "Pérez", "juan@test.com", "123", "12345");

        client.SoftDelete("admin");

        client.IsDeleted.Should().BeTrue();
        client.DeletedAt.Should().NotBeNull();
        client.DeletedBy.Should().Be("admin");
    }

    [Fact]
    public void MarkAsUpdated_ShouldSetUpdatedAtAndUpdatedBy()
    {
        var client = Client.Create("Juan", "Pérez", "juan@test.com", "123", "12345");

        client.MarkAsUpdated("admin");

        client.UpdatedAt.Should().NotBeNull();
        client.UpdatedBy.Should().Be("admin");
    }

    [Fact]
    public void Create_ShouldGenerateUniqueId()
    {
        var client1 = Client.Create("Juan", "Pérez", "juan@test.com", "123", "12345");
        var client2 = Client.Create("María", "García", "maria@test.com", "456", "67890");

        client1.Id.Should().NotBe(client2.Id);
    }
}