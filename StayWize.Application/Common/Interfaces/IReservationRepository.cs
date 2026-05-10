using StayWize.Domain.Entities;

namespace StayWize.Application.Common.Interfaces;

public interface IReservationRepository : IRepository<Reservation>
{
    Task<IEnumerable<Reservation>> GetByPropertyIdAsync(Guid propertyId);
    Task<IEnumerable<Reservation>> GetByClientIdAsync(Guid clientId);
    Task<bool> HasOverlapAsync(Guid propertyId, DateTime checkIn, DateTime checkOut);
    Task<IEnumerable<Reservation>> GetByPropertyIdAndDateRangeAsync(Guid propertyId, DateTime dateFrom, DateTime dateTo);
    Task<IEnumerable<Reservation>> GetByDateRangeAsync(DateTime dateFrom, DateTime dateTo);
}