using StayWize.Domain.Entities;

namespace StayWize.Application.Common.Interfaces;

public interface IAccessLogRepository : IRepository<AccessLog>
{
    Task<IEnumerable<AccessLog>> GetByReservationIdAsync(Guid reservationId);
    Task<IEnumerable<AccessLog>> GetByAccessCodeIdAsync(Guid accessCodeId);
}