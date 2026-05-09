using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;
using StayWize.Services.Encryption;

namespace StayWize.Application.UseCases.Clients;

public record GetAllClientsQuery : IRequest<IEnumerable<ClientDto>>;

public class GetAllClientsQueryHandler
    : IRequestHandler<GetAllClientsQuery, IEnumerable<ClientDto>>
{
    private readonly IClientRepository _repository;
    private readonly IEncryptionService _encryptionService;

    public GetAllClientsQueryHandler(
        IClientRepository repository,
        IEncryptionService encryptionService)
    {
        _repository = repository;
        _encryptionService = encryptionService;
    }

    public async Task<IEnumerable<ClientDto>> Handle(
        GetAllClientsQuery request,
        CancellationToken cancellationToken)
    {
        var clients = await _repository.GetAllAsync();

        return clients.Select(c => new ClientDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            Phone = c.Phone,
            DocumentNumber = _encryptionService.Decrypt(c.DocumentNumber),
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });
    }
}