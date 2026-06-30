using FinTracker.Domain.Models;

namespace FinTracker.Application.Abstractions.Repositories;

public interface IPositionRepository : IRepository<Position>
{
    Task<List<Position>> GetAllByTransactionIdAsync(Guid transactionId);
}
