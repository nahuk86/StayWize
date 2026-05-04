using MediatR;
using StayWize.Application.Common.Interfaces;

namespace StayWize.Application.UseCases.Properties;

public record DeletePropertyCommand(Guid Id) : IRequest<bool>;

public class DeletePropertyCommandHandler
    : IRequestHandler<DeletePropertyCommand, bool>
{
    private readonly IPropertyRepository _repository;

    public DeletePropertyCommandHandler(IPropertyRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeletePropertyCommand request,
        CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.Id);

        if (property is null) return false;

        property.SoftDelete("system");
        await _repository.UpdateAsync(property);

        return true;
    }
}