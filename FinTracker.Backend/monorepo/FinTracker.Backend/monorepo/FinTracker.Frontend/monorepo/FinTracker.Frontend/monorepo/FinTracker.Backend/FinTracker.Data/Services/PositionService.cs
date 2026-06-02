using FinTracker.Domain.Dtos.Positions;
using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Interfaces.Services;
using FinTracker.Domain.Models;

namespace FinTracker.Data.Services;

public class PositionService(IPositionRepository itemRepository, ITransactionRepository transactionRepository) : IPositionService
{
    public async Task<Position> CreateAsync(Guid transactionId, CreatePositionDto dto)
    {
        _ = await transactionRepository.EnsureExistsAsync(transactionId);

        //await ValidateTotalAmount(transactionId, dto.Amount);

        var item = new Position
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Amount = dto.Amount,
            TransactionId = transactionId,
            CategoryId = dto.CategoryId
        };

        await itemRepository.AddAsync(item);
        await itemRepository.SaveChangesAsync();

        return item;
    }

    public async Task DeleteAsync(Guid transactionId, Guid itemId)
    {
        var item = await itemRepository.EnsureExistsAsync(itemId);

        if (item.TransactionId != transactionId)
            throw new ArgumentException($"Item {itemId} does not belong to transaction {transactionId}");

        await itemRepository.DeleteAsync(itemId);
        await itemRepository.SaveChangesAsync();
    }

    public async Task<List<Position>> GetAllAsync(Guid transactionId)
    {
        _ = await transactionRepository.EnsureExistsAsync(transactionId);

        return await itemRepository.GetAllByTransactionIdAsync(transactionId);
    }

    public async Task<Position> UpdateAsync(Guid transactionId, Guid itemId, UpdatePositionDto dto)
    {
        _ = await transactionRepository.EnsureExistsAsync(transactionId);

        var item = await itemRepository.EnsureExistsAsync(itemId);

        // Проверяем что позиция принадлежит этой транзакции
        if (item.TransactionId != transactionId)
            throw new ArgumentException($"Item {itemId} does not belong to transaction {transactionId}");

        if (dto.Name != null)
            item.Name = dto.Name;

        if (dto.Amount != null)
        {
            //await ValidateTotalAmount(transactionId, dto.Amount.Value, excludeItemId: itemId);
            item.Amount = dto.Amount.Value;
        }

        if (dto.CategoryId != null)
            item.CategoryId = dto.CategoryId;

        await itemRepository.UpdateAsync(item);
        await itemRepository.SaveChangesAsync();

        return item;
    }
}
