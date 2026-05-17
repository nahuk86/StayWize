using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.Reservations;

public record GetAllReservationsQuery(string? UserId = null, string? Role = null)
    : IRequest<IEnumerable<ReservationDto>>;

public class GetAllReservationsQueryHandler
    : IRequestHandler<GetAllReservationsQuery, IEnumerable<ReservationDto>>
{
    private readonly IReservationRepository _repository;
    private readonly IPropertyRepository _propertyRepository;

    public GetAllReservationsQueryHandler(
        IReservationRepository repository,
        IPropertyRepository propertyRepository)
    {
        _repository = repository;
        _propertyRepository = propertyRepository;
    }

    public async Task<IEnumerable<ReservationDto>> Handle(
        GetAllReservationsQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.Reservation> reservations;

        if (request.Role == "Admin")
        {
            reservations = await _repository.GetAllAsync();
        }
        else if (request.Role == "Owner" && request.UserId is not null)
        {
            var properties = await _propertyRepository.GetByOwnerIdAsync(request.UserId);
            var propertyIds = properties.Select(p => p.Id);
            reservations = await _repository.GetByPropertyIdsAsync(propertyIds);
        }
        else if (request.Role == "HostLocal" && request.UserId is not null)
        {
            var properties = await _propertyRepository.GetByHostLocalUserIdAsync(request.UserId);
            var propertyIds = properties.Select(p => p.Id);
            reservations = await _repository.GetByPropertyIdsAsync(propertyIds);
        }
        else
        {
            reservations = Enumerable.Empty<Domain.Entities.Reservation>();
        }

        return reservations.Select(r => new ReservationDto
        {
            Id = r.Id,
            PropertyId = r.PropertyId,
            PropertyName = r.Property?.Name ?? string.Empty,
            ClientId = r.ClientId,
            ClientName = r.Client != null
                ? $"{r.Client.FirstName} {r.Client.LastName}"
                : string.Empty,
            HostLocalId = r.HostLocalId,
            CheckIn = r.CheckIn,
            CheckOut = r.CheckOut,
            GuestCount = r.GuestCount,
            Status = r.Status,
            Notes = r.Notes,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        });
    }
}