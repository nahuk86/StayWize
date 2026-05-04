using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.Reservations;

public record GetReservationsByPropertyQuery(Guid PropertyId) : IRequest<IEnumerable<ReservationDto>>;

public class GetReservationsByPropertyQueryHandler
    : IRequestHandler<GetReservationsByPropertyQuery, IEnumerable<ReservationDto>>
{
    private readonly IReservationRepository _repository;

    public GetReservationsByPropertyQueryHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReservationDto>> Handle(
        GetReservationsByPropertyQuery request,
        CancellationToken cancellationToken)
    {
        var reservations = await _repository.GetByPropertyIdAsync(request.PropertyId);

        return reservations.Select(r => new ReservationDto
        {
            Id = r.Id,
            PropertyId = r.PropertyId,
            ClientId = r.ClientId,
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