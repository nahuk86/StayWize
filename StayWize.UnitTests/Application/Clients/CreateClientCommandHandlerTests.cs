using FluentAssertions;
using Moq;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.Clients;
using StayWize.Domain.Entities;
using StayWize.Services.Encryption;
using StayWize.Services.ExceptionHandling;

namespace StayWize.UnitTests.Application.Clients;

public class CreateClientCommandHandlerTests
{
    private readonly Mock<IClientRepository> _clientRepoMock = new();
    private readonly Mock<IEncryptionService> _encryptionServiceMock = new();

    private CreateClientCommandHandler CreateHandler() => new(
        _clientRepoMock.Object,
        _encryptionServiceMock.Object);

    [Fact]
    public async Task Handle_ValidData_ShouldCreateClient()
    {
        _clientRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Client?)null);
        _clientRepoMock.Setup(r => r.GetByDocumentNumberAsync(It.IsAny<string>()))
            .ReturnsAsync((Client?)null);
        _encryptionServiceMock.Setup(e => e.Encrypt(It.IsAny<string>()))
            .Returns("encrypted-doc");
        _encryptionServiceMock.Setup(e => e.Decrypt(It.IsAny<string>()))
            .Returns("12345678");

        var dto = new CreateClientDto
        {
            FirstName = "Juan",
            LastName = "Pérez",
            Email = "juan@test.com",
            Phone = "123456",
            DocumentNumber = "12345678"
        };

        var result = await CreateHandler().Handle(
            new CreateClientCommand(dto), CancellationToken.None);

        result.Should().NotBeNull();
        result.Email.Should().Be("juan@test.com");
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ShouldThrowConflictException()
    {
        var existingClient = Client.Create("Existing", "Client", "juan@test.com", "123", "99999");

        _clientRepoMock.Setup(r => r.GetByEmailAsync("juan@test.com"))
            .ReturnsAsync(existingClient);

        var dto = new CreateClientDto
        {
            FirstName = "Juan",
            LastName = "Pérez",
            Email = "juan@test.com",
            Phone = "123456",
            DocumentNumber = "12345678"
        };

        var action = async () => await CreateHandler().Handle(
            new CreateClientCommand(dto), CancellationToken.None);

        await action.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_DuplicateDocument_ShouldThrowConflictException()
    {
        var existingClient = Client.Create("Existing", "Client", "other@test.com", "123", "12345678");

        _clientRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Client?)null);
        _clientRepoMock.Setup(r => r.GetByDocumentNumberAsync("12345678"))
            .ReturnsAsync(existingClient);

        var dto = new CreateClientDto
        {
            FirstName = "Juan",
            LastName = "Pérez",
            Email = "juan@test.com",
            Phone = "123456",
            DocumentNumber = "12345678"
        };

        var action = async () => await CreateHandler().Handle(
            new CreateClientCommand(dto), CancellationToken.None);

        await action.Should().ThrowAsync<ConflictException>();
    }
}