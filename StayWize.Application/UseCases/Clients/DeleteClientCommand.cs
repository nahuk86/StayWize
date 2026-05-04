using MediatR;
using StayWize.Application.Common.Interfaces;

namespace StayWize.Application.UseCases.Clients;

public record DeleteClientCommand(Guid Id) : IRequest<bool>;

public class DeleteClientCommandHandler
    : IRequestHandler<DeleteClientCommand, bool>
{
    private readonly IClientRepository _repository;

    public DeleteClientCommandHandler(IClientRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeleteClientCommand request,
        CancellationToken cancellationToken)
    {
        var client = await _repository.GetByIdAsync(request.Id);

        if (client is null) return false;

        client.SoftDelete("system");
        await _repository.UpdateAsync(client);

        return true;
    }
}