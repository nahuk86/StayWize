using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.Clients;

public record GetClientByIdQuery(Guid Id) : IRequest<ClientDto?>;

public class GetClientByIdQueryHandler
    : IRequestHandler<GetClientByIdQuery, ClientDto?>
{
    private readonly IClientRepository _repository;

    public GetClientByIdQueryHandler(IClientRepository repository)
    {
        _repository = repository;
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
            DocumentNumber = client.DocumentNumber,
            CreatedAt = client.CreatedAt,
            UpdatedAt = client.UpdatedAt
        };
    }
}