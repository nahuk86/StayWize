using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs;

namespace StayWize.Application.UseCases.Reservations;

public record GetReservationByIdQuery(Guid Id) : IRequest<ReservationDto?>;

public class GetReservationByIdQueryHandler
    : IRequestHandler<GetReservationByIdQuery, ReservationDto?>
{
    private readonly IReservationRepository _repository;

    public GetReservationByIdQueryHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReservationDto?> Handle(
        GetReservationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var reservation = await _repository.GetByIdAsync(request.Id);

        if (reservation is null) return null;

        return new ReservationDto
        {
            Id = reservation.Id,
            PropertyId = reservation.PropertyId,
            ClientId = reservation.ClientId,
            HostLocalId = reservation.HostLocalId,
            CheckIn = reservation.CheckIn,
            CheckOut = reservation.CheckOut,
            GuestCount = reservation.GuestCount,
            Status = reservation.Status,
            Notes = reservation.Notes,
            CreatedAt = reservation.CreatedAt,
            UpdatedAt = reservation.UpdatedAt
        };
    }
}