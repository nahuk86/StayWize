using Microsoft.EntityFrameworkCore;
using StayWize.Application.Common.Interfaces;
using StayWize.Domain.Entities;
using StayWize.Domain.Enums;
using StayWize.Infrastructure.Persistence.Context;
using StayWize.Services.Encryption;

namespace StayWize.Infrastructure.Persistence.Repositories;

public class AccessCodeRepository : BaseRepository<AccessCode>, IAccessCodeRepository
{
    private readonly IEncryptionService _encryptionService;

    public AccessCodeRepository(AppDbContext context, IEncryptionService encryptionService) : base(context)
    {
        _encryptionService = encryptionService;
    }

    public async Task<AccessCode?> GetByPlainCodeAsync(string plainCode)
    {
        var all = await _dbSet.Include(a => a.Reservation).ToListAsync();
        return all.FirstOrDefault(a =>
        {
            try { return _encryptionService.Decrypt(a.Code) == plainCode; }
            catch { return false; }
        });
    }

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

    public async Task<IEnumerable<AccessCode>> GetByReservationIdsAsync(IEnumerable<Guid> reservationIds)
    {
        return await _dbSet
            .Where(a => reservationIds.Contains(a.ReservationId))
            .ToListAsync();
    }
}