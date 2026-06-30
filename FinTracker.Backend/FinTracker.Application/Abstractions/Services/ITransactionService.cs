using FinTracker.Domain.Dtos.Transactions;
using FinTracker.Domain.Dtos.Universal;
using FinTracker.Domain.Models.ModelsToHelp;

namespace FinTracker.Application.Abstractions.Services;

public interface ITransactionService
{
    Task<TransactionDto> GetByIdAsync(Guid id, bool includeDeleted);
    Task<PagedResponse<TransactionDto>> GetFilteredAsync(TransactionFilter filter, bool includeDeleted);
    Task<TransactionDto> CreateAsync(CreateTransactionDto dto);
    Task<TransactionDto> UpdateAsync(Guid id, UpdateTransactionDto dto);
    Task DeleteAsync(Guid id);
    Task BulkUpdateAsync(BulkUpdateDto dto);
    Task SoftDeleteAsync(Guid id);
}
