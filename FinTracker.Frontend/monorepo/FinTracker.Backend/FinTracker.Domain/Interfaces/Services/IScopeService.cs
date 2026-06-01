using FinTracker.Domain.Models;

namespace FinTracker.Data.Services;

public interface IScopeService
{
    Task<List<Scope>> GetAllAsync();
    Task<Scope> GetByIdAsync(Guid id);
    Task<Scope> CreateAsync(string name);
    Task<Scope> UpdateAsync(Guid id, string name);
    Task DeleteAsync(Guid id);
    Task<List<Transaction>> GetTransactionsAsync(Guid scopeId);
}
