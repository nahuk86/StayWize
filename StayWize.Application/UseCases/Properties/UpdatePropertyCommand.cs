using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.Properties;

public record UpdatePropertyCommand(Guid Id, UpdatePropertyDto Dto) : IRequest<bool>;

public class UpdatePropertyCommandHandler
    : IRequestHandler<UpdatePropertyCommand, bool>
{
    private readonly IPropertyRepository _repository;

    public UpdatePropertyCommandHandler(IPropertyRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        UpdatePropertyCommand request,
        CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.Id);

        if (property is null) return false;

        property.Update(
            request.Dto.Name,
            request.Dto.Address,
            request.Dto.City,
            request.Dto.Country,
            request.Dto.MaxGuests);

        property.MarkAsUpdated("system");
        await _repository.UpdateAsync(property);

        return true;
    }
}