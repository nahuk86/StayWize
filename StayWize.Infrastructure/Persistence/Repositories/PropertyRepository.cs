using Microsoft.EntityFrameworkCore;
using StayWize.Application.Common.Interfaces;
using StayWize.Domain.Entities;
using StayWize.Infrastructure.Persistence.Context;

namespace StayWize.Infrastructure.Persistence.Repositories;

public class PropertyRepository : BaseRepository<Property>, IPropertyRepository
{
    public PropertyRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Property>> GetByOwnerIdAsync(string userId)
    {
        // OwnerId en Property es Guid, pero el userId de Identity es string
        // Necesitamos buscar propiedades cuyo OwnerId coincida con el userId parseado
        if (!Guid.TryParse(userId, out var ownerGuid))
            return Enumerable.Empty<Property>();

        return await _dbSet
            .Where(p => p.OwnerId == ownerGuid)
            .ToListAsync();
    }

    public async Task<IEnumerable<Property>> GetByHostLocalUserIdAsync(string userId)
    {
        return await _dbSet
            .Include(p => p.HostLocalAssignments)
                .ThenInclude(a => a.HostLocal)
            .Where(p => p.HostLocalAssignments
                .Any(a => a.HostLocal != null && a.HostLocal.UserId == userId))
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(p => p.Id == id);
    }

    public async Task<Property?> GetByIdWithAssignmentsAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.HostLocalAssignments)
                .ThenInclude(a => a.HostLocal)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AssignHostLocalAsync(Guid propertyId, Guid hostLocalId)
    {
        var exists = await _context.PropertyHostLocals
            .AnyAsync(a => a.PropertyId == propertyId && a.HostLocalId == hostLocalId);

        if (!exists)
        {
            var assignment = PropertyHostLocal.Create(propertyId, hostLocalId);
            await _context.PropertyHostLocals.AddAsync(assignment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnassignHostLocalAsync(Guid propertyId, Guid hostLocalId)
    {
        var assignment = await _context.PropertyHostLocals
            .FirstOrDefaultAsync(a => a.PropertyId == propertyId && a.HostLocalId == hostLocalId);

        if (assignment is not null)
        {
            _context.PropertyHostLocals.Remove(assignment);
            await _context.SaveChangesAsync();
        }
    }
}