using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.HostLocals;

public record UpdateHostLocalCommand(Guid Id, UpdateHostLocalDto Dto) : IRequest<bool>;

public class UpdateHostLocalCommandHandler
    : IRequestHandler<UpdateHostLocalCommand, bool>
{
    private readonly IHostLocalRepository _repository;

    public UpdateHostLocalCommandHandler(IHostLocalRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        UpdateHostLocalCommand request,
        CancellationToken cancellationToken)
    {
        var hostLocal = await _repository.GetByIdAsync(request.Id);

        if (hostLocal is null) return false;

        hostLocal.Update(
            request.Dto.FirstName,
            request.Dto.LastName,
            request.Dto.Email,
            request.Dto.Phone,
            request.Dto.Zone);

        hostLocal.MarkAsUpdated("system");
        await _repository.UpdateAsync(hostLocal);

        return true;
    }
}