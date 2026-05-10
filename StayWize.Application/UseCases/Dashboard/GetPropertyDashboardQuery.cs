using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs.Dashboard;
using StayWize.Domain.Enums;
using StayWize.Services.ExceptionHandling;

namespace StayWize.Application.UseCases.Dashboard;

public record GetPropertyDashboardQuery(
    Guid PropertyId,
    DateTime? DateFrom,
    DateTime? DateTo) : IRequest<PropertyDashboardDto>;

public class GetPropertyDashboardQueryHandler
    : IRequestHandler<GetPropertyDashboardQuery, PropertyDashboardDto>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IAccessCodeRepository _accessCodeRepository;
    private readonly IAccessLogRepository _accessLogRepository;

    public GetPropertyDashboardQueryHandler(
        IPropertyRepository propertyRepository,
        IReservationRepository reservationRepository,
        IAccessCodeRepository accessCodeRepository,
        IAccessLogRepository accessLogRepository)
    {
        _propertyRepository = propertyRepository;
        _reservationRepository = reservationRepository;
        _accessCodeRepository = accessCodeRepository;
        _accessLogRepository = accessLogRepository;
    }

    public async Task<PropertyDashboardDto> Handle(
        GetPropertyDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
        if (property is null)
            throw new NotFoundException("Propiedad", request.PropertyId);

        var dateFrom = request.DateFrom ?? DateTime.UtcNow.AddMonths(-1);
        var dateTo = request.DateTo ?? DateTime.UtcNow;

        var reservations = (await _reservationRepository
            .GetByPropertyIdAndDateRangeAsync(request.PropertyId, dateFrom, dateTo))
            .ToList();

        var reservationIds = reservations.Select(r => r.Id).ToList();

        var accessCodes = reservationIds.Any()
            ? (await _accessCodeRepository.GetByReservationIdsAsync(reservationIds)).ToList()
            : new List<Domain.Entities.AccessCode>();

        var accessLogs = reservationIds.Any()
            ? (await _accessLogRepository.GetByReservationIdsAndDateRangeAsync(
                reservationIds, dateFrom, dateTo)).ToList()
            : new List<Domain.Entities.AccessLog>();

        return new PropertyDashboardDto
        {
            PropertyId = property.Id,
            PropertyName = property.Name,
            DateFrom = dateFrom,
            DateTo = dateTo,
            TotalReservations = reservations.Count,
            ActiveReservations = reservations.Count(r =>
                r.Status == ReservationStatus.Confirmed ||
                r.Status == ReservationStatus.Pending),
            TotalEntries = accessLogs.Count(l => l.EventType == AccessEventType.Entry),
            TotalExits = accessLogs.Count(l => l.EventType == AccessEventType.Exit),
            SuccessfulAccesses = accessLogs.Count(l => l.Success),
            FailedAccesses = accessLogs.Count(l => !l.Success),
            ActiveAccessCodes = accessCodes.Count(c => c.Status == AccessCodeStatus.Active),
            ExpiredAccessCodes = accessCodes.Count(c => c.Status == AccessCodeStatus.Expired),
            RevokedAccessCodes = accessCodes.Count(c => c.Status == AccessCodeStatus.Revoked)
        };
    }
}