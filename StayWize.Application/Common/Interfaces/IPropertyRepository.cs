using StayWize.Domain.Entities;

namespace StayWize.Application.Common.Interfaces;

public interface IPropertyRepository : IRepository<Property>
{
    Task<IEnumerable<Property>> GetByOwnerIdAsync(Guid ownerId);
    Task<bool> ExistsAsync(Guid id);
}