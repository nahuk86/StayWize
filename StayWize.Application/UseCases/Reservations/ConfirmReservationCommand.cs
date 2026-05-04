using MediatR;
using StayWize.Application.Common.Interfaces;

namespace StayWize.Application.UseCases.Reservations;

public record ConfirmReservationCommand(Guid Id) : IRequest<bool>;

public class ConfirmReservationCommandHandler
    : IRequestHandler<ConfirmReservationCommand, bool>
{
    private readonly IReservationRepository _repository;

    public ConfirmReservationCommandHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        ConfirmReservationCommand request,
        CancellationToken cancellationToken)
    {
        var reservation = await _repository.GetByIdAsync(request.Id);
        if (reservation is null) return false;

        reservation.Confirm();
        reservation.MarkAsUpdated("system");
        await _repository.UpdateAsync(reservation);

        return true;
    }
}