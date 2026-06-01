using Microsoft.EntityFrameworkCore;
using StayWize.Application.Common.Interfaces;
using StayWize.Domain.Entities;
using StayWize.Infrastructure.Persistence;  
using StayWize.Infrastructure.Persistence.Context;


namespace StayWize.Infrastructure.Persistence.Repositories;

public class ClientRegistrationRequestRepository : IClientRegistrationRequestRepository
{
    private readonly AppDbContext _context;

    public ClientRegistrationRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ClientRegistrationRequest?> GetByIdAsync(Guid id)
        => await _context.ClientRegistrationRequests.FindAsync(id);

    public async Task<IEnumerable<ClientRegistrationRequest>> GetPendingAsync()
        => await _context.ClientRegistrationRequests
            .Where(r => r.Status == RegistrationRequestStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<bool> ExistsByEmailAsync(string email)
        => await _context.ClientRegistrationRequests
            .AnyAsync(r => r.Email == email
                && r.Status == RegistrationRequestStatus.Pending);

    public async Task<bool> ExistsByDocumentNumberAsync(string documentNumber)
        => await _context.ClientRegistrationRequests
            .AnyAsync(r => r.DocumentNumber == documentNumber
                && r.Status == RegistrationRequestStatus.Pending);

    public async Task AddAsync(ClientRegistrationRequest request)
    {
        await _context.ClientRegistrationRequests.AddAsync(request);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ClientRegistrationRequest request)
    {
        _context.ClientRegistrationRequests.Update(request);
        await _context.SaveChangesAsync();
    }
}