using StayWize.Domain.Entities;

namespace StayWize.Application.Common.Interfaces;

public interface IUserInvitationRepository
{
    Task<UserInvitation?> GetByTokenHashAsync(string tokenHash);
    Task AddAsync(UserInvitation invitation);
    Task UpdateAsync(UserInvitation invitation);
}