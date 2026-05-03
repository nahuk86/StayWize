using StayWize.Domain.Entities;

namespace StayWize.Application.Common.Interfaces;

public interface IAccessCodeRepository : IRepository<AccessCode>
{
    Task<AccessCode?> GetByCodeAsync(string code);
    Task<IEnumerable<AccessCode>> GetByReservationIdAsync(Guid reservationId);
}