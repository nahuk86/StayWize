using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.Clients;

public record GetClientByEmailQuery(string Email) : IRequest<ClientDto?>;

public class GetClientByEmailQueryHandler
    : IRequestHandler<GetClientByEmailQuery, ClientDto?>
{
    private readonly IClientRepository _repository;

    public GetClientByEmailQueryHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClientDto?> Handle(
        GetClientByEmailQuery request,
        CancellationToken cancellationToken)
    {
        var client = await _repository.GetByEmailAsync(request.Email);

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