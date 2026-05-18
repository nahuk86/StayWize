using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Services.Notifications;

namespace StayWize.Application.UseCases.AccessCodes;

public record RevokeAccessCodeCommand(Guid Id) : IRequest<bool>;

public class RevokeAccessCodeCommandHandler
    : IRequestHandler<RevokeAccessCodeCommand, bool>
{
    private readonly IAccessCodeRepository _accessCodeRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IEmailService _emailService;
    private readonly ISmartLockService _smartLockService;

    public RevokeAccessCodeCommandHandler(
        IAccessCodeRepository accessCodeRepository,
        IReservationRepository reservationRepository,
        IClientRepository clientRepository,
        IPropertyRepository propertyRepository,
        IEmailService emailService,
        ISmartLockService smartLockService)
    {
        _accessCodeRepository = accessCodeRepository;
        _reservationRepository = reservationRepository;
        _clientRepository = clientRepository;
        _propertyRepository = propertyRepository;
        _emailService = emailService;
        _smartLockService = smartLockService;
    }

    public async Task<bool> Handle(
        RevokeAccessCodeCommand request,
        CancellationToken cancellationToken)
    {
        var accessCode = await _accessCodeRepository.GetByIdAsync(request.Id);
        if (accessCode is null) return false;

        accessCode.Revoke();
        accessCode.MarkAsUpdated("system");
        await _accessCodeRepository.UpdateAsync(accessCode);

        var reservation = await _reservationRepository.GetByIdAsync(accessCode.ReservationId);
        if (reservation is not null)
        {
            var client = await _clientRepository.GetByIdAsync(reservation.ClientId);
            var property = await _propertyRepository.GetByIdAsync(reservation.PropertyId);

            if (client is not null && property is not null)
            {
                // Notificación al cliente — fire and forget
                _ = _emailService.SendAccessCodeRevokedAsync(
                    client.Email,
                    $"{client.FirstName} {client.LastName}",
                    property.Name);

                // Revocar en cerradura IoT si tiene dispositivo asignado — fire and forget
                if (!string.IsNullOrEmpty(property.LockDeviceId))
                {
                    var plainCode = accessCode.Code; // ya está en claro al momento de revocar
                    _ = _smartLockService.RevokeCodeAsync(property.LockDeviceId, plainCode);
                }
            }
        }

        return true;
    }
}
