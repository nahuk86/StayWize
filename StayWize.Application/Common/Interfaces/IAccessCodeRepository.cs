using StayWize.Domain.Entities;

namespace StayWize.Application.Common.Interfaces;

public interface IAccessCodeRepository : IRepository<AccessCode>
{
    Task<AccessCode?> GetByCodeAsync(string code);
    Task<AccessCode?> GetByPlainCodeAsync(string plainCode);
    Task<IEnumerable<AccessCode>> GetByReservationIdAsync(Guid reservationId);
    Task<IEnumerable<AccessCode>> GetExpiredActiveCodesAsync();
    Task<IEnumerable<AccessCode>> GetByReservationIdsAsync(IEnumerable<Guid> reservationIds);
}