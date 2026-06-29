using FinTracker.Domain.Dtos.Scopes;
using FinTracker.Domain.Dtos.Transactions;

namespace FinTracker.Domain.Interfaces.Services;

public interface IScopeService
{
    Task<List<ScopeDto>> GetAllAsync();
    Task<ScopeDto> GetByIdAsync(Guid id);
    Task<ScopeDto> CreateAsync(string name);
    Task<ScopeDto> UpdateAsync(Guid id, string name);
    Task DeleteAsync(Guid id);
    Task<List<TransactionDto>> GetTransactionsAsync(Guid scopeId);
}
