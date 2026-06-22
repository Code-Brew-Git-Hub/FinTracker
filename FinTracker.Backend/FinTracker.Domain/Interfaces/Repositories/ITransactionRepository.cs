using FinTracker.Domain.Dtos.Transactions;
using FinTracker.Domain.Models;
using FinTracker.Domain.Models.ModelsToHelp;

namespace FinTracker.Domain.Interfaces.Repositories;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<Transaction?> GetByIdAsync(Guid id, bool includeDeleted);
    Task<List<Transaction>> GetFilteredAsync(TransactionFilter filter, bool includeDeleted);
    Task<int> GetFilteredCountAsync(TransactionFilter filter, bool includeDeleted);
    Task BulkUpdateAsync(BulkUpdateDto dto);
    Task<List<Transaction>> GetByScopeIdAsync(Guid scopeId);
    void ClearChangeTracker();
}
