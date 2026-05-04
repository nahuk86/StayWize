using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.Reservations;

public record GetReservationsByClientQuery(Guid ClientId) : IRequest<IEnumerable<ReservationDto>>;

public class GetReservationsByClientQueryHandler
    : IRequestHandler<GetReservationsByClientQuery, IEnumerable<ReservationDto>>
{
    private readonly IReservationRepository _repository;

    public GetReservationsByClientQueryHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReservationDto>> Handle(
        GetReservationsByClientQuery request,
        CancellationToken cancellationToken)
    {
        var reservations = await _repository.GetByClientIdAsync(request.ClientId);

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