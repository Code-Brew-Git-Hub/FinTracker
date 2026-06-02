using FinTracker.Domain.Dtos.TransactionLinks;
using FinTracker.Domain.Models;

namespace FinTracker.Domain.Interfaces.Services;

public interface ILinkService
{
    Task<TransactionLink> GetByIdAsync(Guid id);
    Task<TransactionLink> CreateAsync(CreateTransactionLinkDto dto);
    Task<TransactionLink> AddTransactionAsync(Guid linkId, Guid transactionId);
    Task RemoveTransactionAsync(Guid linkId, Guid transactionId);
    Task DeleteAsync(Guid id);
}
