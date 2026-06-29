using FinTracker.Domain.Dtos.TransactionLinks;
using FinTracker.Domain.Models;

namespace FinTracker.Domain.Interfaces.Services;

public interface ILinkService
{
    Task<TransactionLinkDto> GetByIdAsync(Guid id);
    Task<TransactionLinkDto> CreateAsync(CreateTransactionLinkDto dto);
    Task<TransactionLinkDto> AddTransactionAsync(Guid linkId, Guid transactionId);
    Task RemoveTransactionAsync(Guid linkId, Guid transactionId);
    Task DeleteAsync(Guid id);
}
