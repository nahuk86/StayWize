using Microsoft.EntityFrameworkCore;
using StayWize.Application.Common.Interfaces;
using StayWize.Domain.Entities;
using StayWize.Domain.Enums;
using StayWize.Infrastructure.Persistence.Context;

namespace StayWize.Infrastructure.Persistence.Repositories;

public class AccessCodeRepository : BaseRepository<AccessCode>, IAccessCodeRepository
{
    public AccessCodeRepository(AppDbContext context) : base(context) { }

    public async Task<AccessCode?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Include(a => a.Reservation)
            .FirstOrDefaultAsync(a => a.Code == code);
    }

    public async Task<IEnumerable<AccessCode>> GetByReservationIdAsync(Guid reservationId)
    {
        return await _dbSet
            .Where(a => a.ReservationId == reservationId)
            .ToListAsync();
    }

    public async Task<IEnumerable<AccessCode>> GetExpiredActiveCodesAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(a => a.Status == AccessCodeStatus.Active && a.ValidTo < now)
            .Include(a => a.Reservation)
            .ToListAsync();
    }
}