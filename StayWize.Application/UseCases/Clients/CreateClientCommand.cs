using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Domain.Entities;
using StayWize.Services.Encryption;
using StayWize.Services.ExceptionHandling;

namespace StayWize.Application.UseCases.Clients;

public record CreateClientCommand(CreateClientDto Dto) : IRequest<ClientDto>;

public class CreateClientCommandHandler
    : IRequestHandler<CreateClientCommand, ClientDto>
{
    private readonly IClientRepository _repository;
    private readonly IEncryptionService _encryptionService;

    public CreateClientCommandHandler(
        IClientRepository repository,
        IEncryptionService encryptionService)
    {
        _repository = repository;
        _encryptionService = encryptionService;
    }

    public async Task<ClientDto> Handle(
        CreateClientCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var existingEmail = await _repository.GetByEmailAsync(dto.Email);
        if (existingEmail is not null)
            throw new ConflictException($"Ya existe un cliente con el email {dto.Email}.");

        var existingDocument = await _repository.GetByDocumentNumberAsync(dto.DocumentNumber);
        if (existingDocument is not null)
            throw new ConflictException($"Ya existe un cliente con el documento {dto.DocumentNumber}.");

        // Encriptamos el documento antes de persistir
        var encryptedDocument = _encryptionService.Encrypt(dto.DocumentNumber);

        var client = Client.Create(
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.Phone,
            encryptedDocument);

        await _repository.AddAsync(client);

        return new ClientDto
        {
            Id = client.Id,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Email = client.Email,
            Phone = client.Phone,
            // Desencriptamos al devolver
            DocumentNumber = _encryptionService.Decrypt(client.DocumentNumber),
            CreatedAt = client.CreatedAt,
            UpdatedAt = client.UpdatedAt
        };
    }
}