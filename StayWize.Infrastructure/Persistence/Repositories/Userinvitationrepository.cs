using Microsoft.EntityFrameworkCore;
using StayWize.Application.Common.Interfaces;
using StayWize.Domain.Entities;
using StayWize.Infrastructure.Persistence.Context;

namespace StayWize.Infrastructure.Persistence.Repositories;

public class UserInvitationRepository : IUserInvitationRepository
{
    private readonly AppDbContext _context;

    public UserInvitationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserInvitation?> GetByTokenHashAsync(string tokenHash)
    {
        return await _context.UserInvitations
            .FirstOrDefaultAsync(i => i.TokenHash == tokenHash);
    }

    public async Task AddAsync(UserInvitation invitation)
    {
        await _context.UserInvitations.AddAsync(invitation);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserInvitation invitation)
    {
        _context.UserInvitations.Update(invitation);
        await _context.SaveChangesAsync();
    }
}