using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.Clients;

public record GetClientByDocumentQuery(string DocumentNumber) : IRequest<ClientDto?>;

public class GetClientByDocumentQueryHandler
    : IRequestHandler<GetClientByDocumentQuery, ClientDto?>
{
    private readonly IClientRepository _repository;

    public GetClientByDocumentQueryHandler(IClientRepository repository)
    {
        _repository = repository;
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
            DocumentNumber = client.DocumentNumber,
            CreatedAt = client.CreatedAt,
            UpdatedAt = client.UpdatedAt
        };
    }
}