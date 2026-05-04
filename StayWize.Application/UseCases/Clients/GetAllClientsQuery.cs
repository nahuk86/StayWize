using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.Clients;

public record GetAllClientsQuery : IRequest<IEnumerable<ClientDto>>;

public class GetAllClientsQueryHandler
    : IRequestHandler<GetAllClientsQuery, IEnumerable<ClientDto>>
{
    private readonly IClientRepository _repository;

    public GetAllClientsQueryHandler(IClientRepository repository)
    {
        _repository = repository;
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
            DocumentNumber = c.DocumentNumber,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });
    }
}