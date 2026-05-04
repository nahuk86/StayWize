using MediatR;
using StayWize.Application.Common.Interfaces;

namespace StayWize.Application.UseCases.Reservations;

public record DeleteReservationCommand(Guid Id) : IRequest<bool>;

public class DeleteReservationCommandHandler
    : IRequestHandler<DeleteReservationCommand, bool>
{
    private readonly IReservationRepository _repository;

    public DeleteReservationCommandHandler(IReservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeleteReservationCommand request,
        CancellationToken cancellationToken)
    {
        var reservation = await _repository.GetByIdAsync(request.Id);
        if (reservation is null) return false;

        reservation.SoftDelete("system");
        await _repository.UpdateAsync(reservation);

        return true;
    }
}