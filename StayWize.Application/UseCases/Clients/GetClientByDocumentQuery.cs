using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Services.Encryption;


namespace StayWize.Application.UseCases.Clients;

public record GetClientByDocumentQuery(string DocumentNumber) : IRequest<ClientDto?>;

public class GetClientByDocumentQueryHandler
    : IRequestHandler<GetClientByDocumentQuery, ClientDto?>
{
    private readonly IClientRepository _repository;
    private readonly IEncryptionService _encryptionService;

    public GetClientByDocumentQueryHandler(IClientRepository repository, IEncryptionService encryptionService)
    {
        _repository = repository;
        _encryptionService = encryptionService;

    }

    public async Task<ClientDto?> Handle(
        GetClientByDocumentQuery request,
        CancellationToken cancellationToken)
    {
        var client = await _repository.GetByDocumentNumberAsync(request.DocumentNumber);

        if (client is null) return null;

        return new ClientDto
        {
            Id = client.Id,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Email = client.Email,
            Phone = client.Phone,
            DocumentNumber = _encryptionService.Decrypt(client.DocumentNumber),
            CreatedAt = client.CreatedAt,
            UpdatedAt = client.UpdatedAt
        };
    }
}