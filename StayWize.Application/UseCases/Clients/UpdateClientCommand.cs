using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.Clients;

public record UpdateClientCommand(Guid Id, UpdateClientDto Dto) : IRequest<bool>;

public class UpdateClientCommandHandler
    : IRequestHandler<UpdateClientCommand, bool>
{
    private readonly IClientRepository _repository;

    public UpdateClientCommandHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        UpdateClientCommand request,
        CancellationToken cancellationToken)
    {
        var client = await _repository.GetByIdAsync(request.Id);

        if (client is null) return false;

        client.Update(
            request.Dto.FirstName,
            request.Dto.LastName,
            request.Dto.Email,
            request.Dto.Phone);

        client.MarkAsUpdated("system");
        await _repository.UpdateAsync(client);

        return true;
    }
}