using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.Common.Services;
using StayWize.Application.DTOs;
using StayWize.Domain.Entities;
using StayWize.Domain.Exceptions;
using StayWize.Services.ExceptionHandling;

namespace StayWize.Application.UseCases.Reservations;

public record CreateReservationCommand(CreateReservationDto Dto) : IRequest<ReservationDto>;

public class CreateReservationCommandHandler
    : IRequestHandler<CreateReservationCommand, ReservationDto>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ReservationConcurrencyService _concurrencyService;

    public CreateReservationCommandHandler(
        IReservationRepository reservationRepository,
        IPropertyRepository propertyRepository,
        IClientRepository clientRepository,
        ReservationConcurrencyService concurrencyService)
    {
        _reservationRepository = reservationRepository;
        _propertyRepository = propertyRepository;
        _clientRepository = clientRepository;
        _concurrencyService = concurrencyService;
    }

    public async Task<ReservationDto> Handle(
        CreateReservationCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Validar existencia de propiedad y cliente
        var propertyExists = await _propertyRepository.ExistsAsync(dto.PropertyId);
        if (!propertyExists)
            throw new NotFoundException("Propiedad", dto.PropertyId);

        var clientExists = await _clientRepository.ExistsAsync(dto.ClientId);
        if (!clientExists)
            throw new NotFoundException("Cliente", dto.ClientId);

        // Adquirir lock sobre la propiedad (Opción C)
        using var propertyLock = await _concurrencyService.AcquirePropertyLockAsync(dto.PropertyId);

        // Validar solapamiento de fechas (dentro del lock)
        var hasOverlap = await _reservationRepository.HasOverlapAsync(
            dto.PropertyId, dto.CheckIn, dto.CheckOut);

        if (hasOverlap)
            throw new ConflictException("La propiedad ya tiene una reserva activa en el rango de fechas indicado.");

        var reservation = Reservation.Create(
            dto.PropertyId,
            dto.ClientId,
            dto.CheckIn,
            dto.CheckOut,
            dto.GuestCount,
            dto.Notes);

        // Opción A: EF Core detectará conflictos de RowVersion al guardar
        try
        {
            await _reservationRepository.AddAsync(reservation);
        }
        catch (Exception ex) when (ex.GetType().Name == "DbUpdateConcurrencyException")
        {
            throw new ConcurrencyException(
                "La reserva no pudo completarse por un conflicto de concurrencia. Intentá nuevamente.");
        }

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