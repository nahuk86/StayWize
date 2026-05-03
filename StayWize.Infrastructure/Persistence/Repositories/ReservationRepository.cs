using Microsoft.EntityFrameworkCore;
using StayWize.Application.Common.Interfaces;
using StayWize.Domain.Entities;
using StayWize.Domain.Enums;
using StayWize.Infrastructure.Persistence.Context;

namespace StayWize.Infrastructure.Persistence.Repositories;

public class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Reservation>> GetByPropertyIdAsync(Guid propertyId)
    {
        return await _dbSet
            .Where(r => r.PropertyId == propertyId)
            .Include(r => r.Client)
            .Include(r => r.HostLocal)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetByClientIdAsync(Guid clientId)
    {
        return await _dbSet
            .Where(r => r.ClientId == clientId)
            .Include(r => r.Property)
            .ToListAsync();
    }

    public async Task<bool> HasOverlapAsync(Guid propertyId, DateTime checkIn, DateTime checkOut)
    {
        return await _dbSet.AnyAsync(r =>
            r.PropertyId == propertyId &&
            r.Status != ReservationStatus.Cancelled &&
            r.CheckIn < checkOut &&
            r.CheckOut > checkIn);
    }
}