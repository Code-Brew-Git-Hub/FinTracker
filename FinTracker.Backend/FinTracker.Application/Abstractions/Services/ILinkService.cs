using FinTracker.Domain.Dtos.TransactionLinks;

namespace FinTracker.Application.Abstractions.Services;

public interface ILinkService
{
    Task<TransactionLinkDto> GetByIdAsync(Guid id);
    Task<TransactionLinkDto> CreateAsync(CreateTransactionLinkDto dto);
    Task<TransactionLinkDto> AddTransactionAsync(Guid linkId, Guid transactionId);
    Task RemoveTransactionAsync(Guid linkId, Guid transactionId);
    Task DeleteAsync(Guid id);
}
