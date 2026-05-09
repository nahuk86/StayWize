using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Services.Notifications;

namespace StayWize.Application.UseCases.Reservations;

public record ConfirmReservationCommand(Guid Id) : IRequest<bool>;

public class ConfirmReservationCommandHandler
    : IRequestHandler<ConfirmReservationCommand, bool>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IEmailService _emailService;

    public ConfirmReservationCommandHandler(
        IReservationRepository reservationRepository,
        IClientRepository clientRepository,
        IPropertyRepository propertyRepository,
        IEmailService emailService)
    {
        _reservationRepository = reservationRepository;
        _clientRepository = clientRepository;
        _propertyRepository = propertyRepository;
        _emailService = emailService;
    }

    public async Task<bool> Handle(
        ConfirmReservationCommand request,
        CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetByIdAsync(request.Id);
        if (reservation is null) return false;

        reservation.Confirm();
        reservation.MarkAsUpdated("system");
        await _reservationRepository.UpdateAsync(reservation);

        // Notificación fire and forget
        var client = await _clientRepository.GetByIdAsync(reservation.ClientId);
        var property = await _propertyRepository.GetByIdAsync(reservation.PropertyId);

        if (client is not null && property is not null)
        {
            _ = _emailService.SendReservationConfirmedAsync(
                client.Email,
                $"{client.FirstName} {client.LastName}",
                property.Name,
                reservation.CheckIn,
                reservation.CheckOut);
        }

        return true;
    }
}