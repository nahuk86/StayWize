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
            dto.IsSelfCheckIn);

        await _repository.AddAsync(property);

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
            OwnerId = property.OwnerId,
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };
    }
}