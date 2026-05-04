using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.Properties;

public record GetAllPropertiesQuery : IRequest<IEnumerable<PropertyDto>>;

public class GetAllPropertiesQueryHandler
    : IRequestHandler<GetAllPropertiesQuery, IEnumerable<PropertyDto>>
{
    private readonly IPropertyRepository _repository;

    public GetAllPropertiesQueryHandler(IPropertyRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PropertyDto>> Handle(
        GetAllPropertiesQuery request,
        CancellationToken cancellationToken)
    {
        var properties = await _repository.GetAllAsync();

        return properties.Select(p => new PropertyDto
        {
            Id = p.Id,
            Name = p.Name,
            Address = p.Address,
            City = p.City,
            Country = p.Country,
            MaxGuests = p.MaxGuests,
            IsActive = p.IsActive,
            OwnerId = p.OwnerId,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });
    }
}