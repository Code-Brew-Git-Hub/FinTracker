using FinTracker.Domain.Models;

namespace FinTracker.Application.Abstractions.Repositories;

public interface ILinkRepository : IRepository<TransactionLink>
{
    Task<TransactionLink?> GetByIdWithEntriesAsync(Guid id);
}
