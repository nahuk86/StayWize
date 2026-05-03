using Microsoft.EntityFrameworkCore;
using StayWize.Application.Common.Interfaces;
using StayWize.Domain.Entities;
using StayWize.Infrastructure.Persistence.Context;

namespace StayWize.Infrastructure.Persistence.Repositories;

public class ClientRepository : BaseRepository<Client>, IClientRepository
{
    public ClientRepository(AppDbContext context) : base(context) { }

    public async Task<Client?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Client?> GetByDocumentNumberAsync(string documentNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.DocumentNumber == documentNumber);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(c => c.Id == id);
    }
}