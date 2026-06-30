using FinTracker.Application.Abstractions.Repositories;
using FinTracker.Application.Abstractions.Services;
using FinTracker.Domain.Dtos.Positions;
using FinTracker.Domain.Models;
using MapsterMapper;

namespace FinTracker.Application.Services;

public class PositionService(IPositionRepository itemRepository,
    ITransactionRepository transactionRepository,
    IMapper mapper) : IPositionService
{
    public async Task<PositionDto> CreateAsync(Guid transactionId, CreatePositionDto dto)
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

        return mapper.Map<PositionDto>(item);
    }

    public async Task DeleteAsync(Guid transactionId, Guid itemId)
    {
        var item = await itemRepository.EnsureExistsAsync(itemId);

        if (item.TransactionId != transactionId)
            throw new ArgumentException($"Item {itemId} does not belong to transaction {transactionId}");

        await itemRepository.DeleteAsync(itemId);
        await itemRepository.SaveChangesAsync();
    }

    public async Task<List<PositionDto>> GetAllAsync(Guid transactionId)
    {
        _ = await transactionRepository.EnsureExistsAsync(transactionId);

        var position = await itemRepository.GetAllByTransactionIdAsync(transactionId);
        return mapper.Map<List<PositionDto>>(position);
    }

    public async Task<PositionDto> UpdateAsync(Guid transactionId, Guid itemId, UpdatePositionDto dto)
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
            item.Amount = dto.Amount.Value;
        }

        if (dto.CategoryId != null)
            item.CategoryId = dto.CategoryId;

        await itemRepository.UpdateAsync(item);
        await itemRepository.SaveChangesAsync();

        return mapper.Map<PositionDto>(item);
    }
}
