using FinTracker.Domain.Models;

namespace FinTracker.Domain.Interfaces.Repositories;

public interface ILinkRepository : IRepository<TransactionLink>
{
    Task<TransactionLink?> GetByIdWithEntriesAsync(Guid id);
}
