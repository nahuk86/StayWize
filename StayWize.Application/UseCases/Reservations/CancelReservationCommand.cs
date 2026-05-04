using MediatR;
using StayWize.Application.Common.Interfaces;

namespace StayWize.Application.UseCases.Reservations;

public record CancelReservationCommand(Guid Id) : IRequest<bool>;

public class CancelReservationCommandHandler
    : IRequestHandler<CancelReservationCommand, bool>
{
    private readonly IReservationRepository _repository;

    public CancelReservationCommandHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        CancelReservationCommand request,
        CancellationToken cancellationToken)
    {
        var reservation = await _repository.GetByIdAsync(request.Id);
        if (reservation is null) return false;

        reservation.Cancel();
        reservation.MarkAsUpdated("system");
        await _repository.UpdateAsync(reservation);

        return true;
    }
}