using MediatR;
using StayWize.Application.Common.Interfaces;

namespace StayWize.Application.UseCases.HostLocals;

public record SetHostLocalAvailabilityCommand(Guid Id, bool IsAvailable) : IRequest<bool>;

public class SetHostLocalAvailabilityCommandHandler
    : IRequestHandler<SetHostLocalAvailabilityCommand, bool>
{
    private readonly IHostLocalRepository _repository;

    public SetHostLocalAvailabilityCommandHandler(IHostLocalRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        SetHostLocalAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        var hostLocal = await _repository.GetByIdAsync(request.Id);

        if (hostLocal is null) return false;

        hostLocal.SetAvailability(request.IsAvailable);
        hostLocal.MarkAsUpdated("system");
        await _repository.UpdateAsync(hostLocal);

        return true;
    }
}