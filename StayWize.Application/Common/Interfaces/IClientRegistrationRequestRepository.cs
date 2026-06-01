using StayWize.Domain.Entities;

namespace StayWize.Application.Common.Interfaces;

public interface IClientRegistrationRequestRepository
{
    Task<ClientRegistrationRequest?> GetByIdAsync(Guid id);
    Task<IEnumerable<ClientRegistrationRequest>> GetPendingAsync();
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByDocumentNumberAsync(string documentNumber);
    Task AddAsync(ClientRegistrationRequest request);
    Task UpdateAsync(ClientRegistrationRequest request);
}