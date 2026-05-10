using Microsoft.EntityFrameworkCore;
using StayWize.Application.Common.Interfaces;
using StayWize.Domain.Entities;
using StayWize.Infrastructure.Persistence.Context;

namespace StayWize.Infrastructure.Persistence.Repositories;

public class AccessLogRepository : BaseRepository<AccessLog>, IAccessLogRepository
{
    public AccessLogRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<AccessLog>> GetByReservationIdAsync(Guid reservationId)
    {
        return await _dbSet
            .Where(l => l.ReservationId == reservationId)
            .OrderByDescending(l => l.EventTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<AccessLog>> GetByAccessCodeIdAsync(Guid accessCodeId)
    {
        return await _dbSet
            .Where(l => l.AccessCodeId == accessCodeId)
            .OrderByDescending(l => l.EventTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<AccessLog>> GetByReservationIdsAndDateRangeAsync(
    IEnumerable<Guid> reservationIds, DateTime dateFrom, DateTime dateTo)
    {
        return await _dbSet
            .Where(l => reservationIds.Contains(l.ReservationId) &&
                        l.EventTime >= dateFrom &&
                        l.EventTime <= dateTo)
            .OrderByDescending(l => l.EventTime)
            .ToListAsync();
    }
}