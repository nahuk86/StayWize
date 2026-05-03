using StayWize.Domain.Entities;

namespace StayWize.Application.Common.Interfaces;

public interface IClientRepository : IRepository<Client>
{
    Task<Client?> GetByEmailAsync(string email);
    Task<Client?> GetByDocumentNumberAsync(string documentNumber);
    Task<bool> ExistsAsync(Guid id);
}