using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.Properties;

public record GetAllPropertiesQuery(string? UserId = null, string? Role = null)
    : IRequest<IEnumerable<PropertyDto>>;

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
        IEnumerable<Domain.Entities.Property> properties;

        if (request.Role == "Admin")
        {
            properties = await _repository.GetAllAsync();
        }
        else if (request.Role == "Owner" && request.UserId is not null)
        {
            properties = await _repository.GetByOwnerIdAsync(request.UserId);
        }
        else if (request.Role == "HostLocal" && request.UserId is not null)
        {
            properties = await _repository.GetByHostLocalUserIdAsync(request.UserId);
        }
        else
        {
            properties = Enumerable.Empty<Domain.Entities.Property>();
        }

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