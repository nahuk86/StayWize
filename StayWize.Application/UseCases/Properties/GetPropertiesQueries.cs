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
            properties = await _repository.GetAllAsync();
        else if (request.Role == "Owner" && request.UserId is not null)
            properties = await _repository.GetByOwnerIdAsync(request.UserId);
        else if (request.Role == "HostLocal" && request.UserId is not null)
            properties = await _repository.GetByHostLocalUserIdAsync(request.UserId);
        else
            properties = Enumerable.Empty<Domain.Entities.Property>();

        return properties.Select(MapToDto);
    }

    private static PropertyDto MapToDto(Domain.Entities.Property p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Address = p.Address,
        City = p.City,
        Country = p.Country,
        MaxGuests = p.MaxGuests,
        IsActive = p.IsActive,
        IsSelfCheckIn = p.IsSelfCheckIn,
        LockDeviceId = p.LockDeviceId,
        OwnerId = p.OwnerId,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}

public record GetPropertyByIdQuery(Guid Id) : IRequest<PropertyDto?>;

public class GetPropertyByIdQueryHandler
    : IRequestHandler<GetPropertyByIdQuery, PropertyDto?>
{
    private readonly IPropertyRepository _repository;

    public GetPropertyByIdQueryHandler(IPropertyRepository repository)
    {
        _repository = repository;
    }

    public async Task<PropertyDto?> Handle(
        GetPropertyByIdQuery request,
        CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.Id);
        if (property is null) return null;

        return new PropertyDto
        {
            Id = property.Id,
            Name = property.Name,
            Address = property.Address,
            City = property.City,
            Country = property.Country,
            MaxGuests = property.MaxGuests,
            IsActive = property.IsActive,
            IsSelfCheckIn = property.IsSelfCheckIn,
            LockDeviceId = property.LockDeviceId,
            OwnerId = property.OwnerId,
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };
    }
}
