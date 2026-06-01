using FluentAssertions;
using Moq;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Application.UseCases.Clients;
using StayWize.Domain.Entities;
using StayWize.Services.Encryption;
using StayWize.Services.ExceptionHandling;
using StayWize.Services.Notifications;

namespace StayWize.UnitTests.Application.Clients;

public class CreateClientCommandHandlerTests
{
    private readonly Mock<IClientRepository>          _clientRepoMock      = new();
    private readonly Mock<IEncryptionService>          _encryptionMock      = new();
    private readonly Mock<IUserInvitationRepository>  _invitationRepoMock  = new();
    private readonly Mock<IEmailService>              _emailServiceMock    = new();

    private CreateClientCommandHandler CreateHandler() => new(
        _clientRepoMock.Object,
        _encryptionMock.Object,
        _invitationRepoMock.Object,
        _emailServiceMock.Object);

    [Fact]
    public async Task Handle_ValidData_ShouldCreateClientAndQueueInvitation()
    {
        // Arrange
        _clientRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Client?)null);
        _clientRepoMock.Setup(r => r.GetByDocumentNumberAsync(It.IsAny<string>()))
            .ReturnsAsync((Client?)null);
        _encryptionMock.Setup(e => e.Encrypt(It.IsAny<string>())).Returns("enc");
        _encryptionMock.Setup(e => e.Decrypt(It.IsAny<string>())).Returns("12345678");

        var dto = new CreateClientDto
        {
            FirstName      = "Juan",
            LastName       = "Pérez",
            Email          = "juan@test.com",
            DocumentNumber = "12345678"
        };

        // Act
        var result = await CreateHandler().Handle(
            new CreateClientCommand(dto), CancellationToken.None);

        // Assert
        result.Email.Should().Be("juan@test.com");

        // Verificar que se persistió la invitación
        _invitationRepoMock.Verify(
            r => r.AddAsync(It.Is<UserInvitation>(i =>
                i.Email == dto.Email && i.Role == "Guest")),
            Times.Once);
    }
}