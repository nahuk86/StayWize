using MediatR;
using StayWize.Application.Common.Interfaces;
using StayWize.Application.DTOs.Dashboard;
using StayWize.Domain.Enums;

namespace StayWize.Application.UseCases.Dashboard;

public record GetGlobalDashboardQuery(
    DateTime? DateFrom,
    DateTime? DateTo) : IRequest<GlobalDashboardDto>;

public class GetGlobalDashboardQueryHandler
    : IRequestHandler<GetGlobalDashboardQuery, GlobalDashboardDto>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IAccessLogRepository _accessLogRepository;

    public GetGlobalDashboardQueryHandler(
        IPropertyRepository propertyRepository,
        IClientRepository clientRepository,
        IReservationRepository reservationRepository,
        IAccessLogRepository accessLogRepository)
    {
        _propertyRepository = propertyRepository;
        _clientRepository = clientRepository;
        _reservationRepository = reservationRepository;
        _accessLogRepository = accessLogRepository;
    }

    public async Task<GlobalDashboardDto> Handle(
        GetGlobalDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var dateFrom = request.DateFrom ?? DateTime.UtcNow.AddMonths(-1);
        var dateTo = request.DateTo ?? DateTime.UtcNow;

        var properties = await _propertyRepository.GetAllAsync();
        var clients = await _clientRepository.GetAllAsync();
        var reservations = (await _reservationRepository
            .GetByDateRangeAsync(dateFrom, dateTo)).ToList();

        var reservationIds = reservations.Select(r => r.Id).ToList();

        var accessLogs = reservationIds.Any()
            ? (await _accessLogRepository.GetByReservationIdsAndDateRangeAsync(
                reservationIds, dateFrom, dateTo)).ToList()
            : new List<Domain.Entities.AccessLog>();

        return new GlobalDashboardDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            TotalProperties = properties.Count(),
            TotalClients = clients.Count(),
            TotalReservations = reservations.Count,
            TotalEntries = accessLogs.Count(l => l.EventType == AccessEventType.Entry),
            TotalExits = accessLogs.Count(l => l.EventType == AccessEventType.Exit),
            SuccessfulAccesses = accessLogs.Count(l => l.Success),
            FailedAccesses = accessLogs.Count(l => !l.Success)
        };
    }
}