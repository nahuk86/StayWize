using StayWize.Domain.Entities;

namespace StayWize.Application.Common.Interfaces;

public interface IHostLocalRepository : IRepository<HostLocal>
{
    Task<IEnumerable<HostLocal>> GetAvailableByZoneAsync(string zone);
    Task<bool> ExistsAsync(Guid id);
}