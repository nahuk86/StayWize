using Microsoft.EntityFrameworkCore;
using StayWize.Application.Common.Interfaces;
using StayWize.Domain.Entities;
using StayWize.Infrastructure.Persistence.Context;

namespace StayWize.Infrastructure.Persistence.Repositories;

public class PropertyRepository : BaseRepository<Property>, IPropertyRepository
{
    public PropertyRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _dbSet
            .Where(p => p.OwnerId == ownerId)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(p => p.Id == id);
    }
}