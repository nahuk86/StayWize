using StayWize.Domain.Entities;

namespace StayWize.Application.Common.Interfaces;

public interface IPropertyRepository : IRepository<Property>
{
    Task<IEnumerable<Property>> GetByOwnerIdAsync(string userId);
    Task<IEnumerable<Property>> GetByHostLocalUserIdAsync(string userId);
    Task<bool> ExistsAsync(Guid id);
    Task<Property?> GetByIdWithAssignmentsAsync(Guid id);
    Task AssignHostLocalAsync(Guid propertyId, Guid hostLocalId);
    Task UnassignHostLocalAsync(Guid propertyId, Guid hostLocalId);
}