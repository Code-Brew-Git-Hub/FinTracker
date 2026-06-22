using FinTracker.Domain.Dtos.Transactions;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Models;
using FinTracker.Domain.Models.ModelsToHelp;

namespace FinTracker.Data.Services;

public interface ITransactionService
{
    Task<Transaction> GetByIdAsync(Guid id, bool includeDeleted);
    Task<PagedResponse<Transaction>> GetFilteredAsync(TransactionFilter filter, bool includeDeleted);
    Task<Transaction> CreateAsync(CreateTransactionDto dto);
    Task<Transaction> UpdateAsync(Guid id, UpdateTransactionDto dto);
    Task DeleteAsync(Guid id);
    Task BulkUpdateAsync(BulkUpdateDto dto);
    Task SoftDeleteAsync(Guid id);
}
