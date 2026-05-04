using MediatR;
using StayWize.Application.Common.Interfaces;

namespace StayWize.Application.UseCases.HostLocals;

public record DeleteHostLocalCommand(Guid Id) : IRequest<bool>;

public class DeleteHostLocalCommandHandler
    : IRequestHandler<DeleteHostLocalCommand, bool>
{
    private readonly IHostLocalRepository _repository;

    public DeleteHostLocalCommandHandler(IHostLocalRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeleteHostLocalCommand request,
        CancellationToken cancellationToken)
    {
        var hostLocal = await _repository.GetByIdAsync(request.Id);

        if (hostLocal is null) return false;

        hostLocal.SoftDelete("system");
        await _repository.UpdateAsync(hostLocal);

        return true;
    }
}