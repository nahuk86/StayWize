using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Services.Encryption;


namespace StayWize.Application.UseCases.Clients;

public record GetClientByIdQuery(Guid Id) : IRequest<ClientDto?>;

public class GetClientByIdQueryHandler
    : IRequestHandler<GetClientByIdQuery, ClientDto?>
{
    private readonly IClientRepository _repository;
    private readonly IEncryptionService _encryptionService;


    public GetClientByIdQueryHandler(IClientRepository repository, IEncryptionService encryptionService)
    {
        _repository = repository;
        _encryptionService = encryptionService;
    }

    public async Task<ClientDto?> Handle(
        GetClientByIdQuery request,
        CancellationToken cancellationToken)
    {
        var client = await _repository.GetByIdAsync(request.Id);

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