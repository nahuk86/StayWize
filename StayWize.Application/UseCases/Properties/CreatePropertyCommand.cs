using MediatR;
using StayWize.Application.DTOs;
using StayWize.Application.Common.Interfaces;
using StayWize.Domain.Entities;

namespace StayWize.Application.UseCases.Properties;

public record CreatePropertyCommand(CreatePropertyDto Dto) : IRequest<PropertyDto>;

public class CreatePropertyCommandHandler
    : IRequestHandler<CreatePropertyCommand, PropertyDto>
{
    private readonly IPropertyRepository _repository;

    public CreatePropertyCommandHandler(IPropertyRepository repository)
    {
        _repository = repository;
    }

    public async Task<PropertyDto> Handle(
        CreatePropertyCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var property = Property.Create(
            dto.Name,
            dto.Address,
            dto.City,
            dto.Country,
            dto.MaxGuests,
            dto.OwnerId,
            dto.IsSelfCheckIn,
            dto.LockDeviceId);

        await _repository.AddAsync(property);

        return MapToDto(property);
    }

    private static PropertyDto MapToDto(Property p) => new()
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
