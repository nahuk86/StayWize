using MediatR;
using StayWize.Application.Common.Interfaces;

namespace StayWize.Application.UseCases.AccessCodes;

public record RevokeAccessCodeCommand(Guid Id) : IRequest<bool>;

public class RevokeAccessCodeCommandHandler
    : IRequestHandler<RevokeAccessCodeCommand, bool>
{
    private readonly IAccessCodeRepository _repository;

    public RevokeAccessCodeCommandHandler(IAccessCodeRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        RevokeAccessCodeCommand request,
        CancellationToken cancellationToken)
    {
        var accessCode = await _repository.GetByIdAsync(request.Id);

        if (accessCode is null) return false;

        accessCode.Revoke();
        accessCode.MarkAsUpdated("system");
        await _repository.UpdateAsync(accessCode);

        return true;
    }
}