using FinTracker.Domain.Models;

namespace FinTracker.Domain.Interfaces.Repositories;

public interface IPositionRepository : IRepository<Position>
{
    Task<List<Position>> GetAllByTransactionIdAsync(Guid transactionId);
}
