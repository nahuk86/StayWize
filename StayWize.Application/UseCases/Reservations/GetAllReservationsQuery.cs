using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.Reservations;

public record GetAllReservationsQuery : IRequest<IEnumerable<ReservationDto>>;

public class GetAllReservationsQueryHandler
    : IRequestHandler<GetAllReservationsQuery, IEnumerable<ReservationDto>>
{
    private readonly IReservationRepository _repository;

    public GetAllReservationsQueryHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReservationDto>> Handle(
        GetAllReservationsQuery request,
        CancellationToken cancellationToken)
    {
        var reservations = await _repository.GetAllAsync();

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