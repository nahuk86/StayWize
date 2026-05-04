using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.Common.Services;
using StayWize.Domain.Exceptions;

namespace StayWize.Application.UseCases.Reservations;

public record AssignHostLocalCommand(Guid ReservationId, Guid HostLocalId) : IRequest<bool>;

public class AssignHostLocalCommandHandler
    : IRequestHandler<AssignHostLocalCommand, bool>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IHostLocalRepository _hostLocalRepository;
    private readonly ReservationConcurrencyService _concurrencyService;

    public AssignHostLocalCommandHandler(
        IReservationRepository reservationRepository,
        IHostLocalRepository hostLocalRepository,
        ReservationConcurrencyService concurrencyService)
    {
        _reservationRepository = reservationRepository;
        _hostLocalRepository = hostLocalRepository;
        _concurrencyService = concurrencyService;
    }

    public async Task<bool> Handle(
        AssignHostLocalCommand request,
        CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId);
        if (reservation is null) return false;

        var hostLocal = await _hostLocalRepository.GetByIdAsync(request.HostLocalId);
        if (hostLocal is null)
            throw new InvalidOperationException($"El host local {request.HostLocalId} no existe.");

        // Adquirir lock sobre el host local (Opción C)
        using var hostLock = await _concurrencyService.AcquireHostLocalLockAsync(request.HostLocalId);

        if (!hostLocal.IsAvailable)
            throw new InvalidOperationException("El host local no está disponible.");

        reservation.AssignHostLocal(request.HostLocalId);
        reservation.MarkAsUpdated("system");

        try
        {
            await _reservationRepository.UpdateAsync(reservation);
        }
        catch (Exception ex) when (ex.GetType().Name == "DbUpdateConcurrencyException")
        {
            throw new ConcurrencyException(
                "El host local no pudo asignarse por un conflicto de concurrencia. Intentá nuevamente.");
        }

        return true;
    }
}