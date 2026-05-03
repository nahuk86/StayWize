using Microsoft.EntityFrameworkCore;
using StayWize.Application.Common.Interfaces;
using StayWize.Domain.Entities;
using StayWize.Infrastructure.Persistence.Context;

namespace StayWize.Infrastructure.Persistence.Repositories;

public class HostLocalRepository : BaseRepository<HostLocal>, IHostLocalRepository
{
    public HostLocalRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<HostLocal>> GetAvailableByZoneAsync(string zone)
    {
        return await _dbSet
            .Where(h => h.Zone == zone && h.IsAvailable)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(h => h.Id == id);
    }
}