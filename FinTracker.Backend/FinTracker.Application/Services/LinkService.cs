using FinTracker.Application.Abstractions.Repositories;
using FinTracker.Application.Abstractions.Services;
using FinTracker.Domain.Dtos.TransactionLinks;
using FinTracker.Domain.Models;
using MapsterMapper;

namespace FinTracker.Application.Services;

public class LinkService(ILinkRepository linkRepository,
    ITransactionRepository transactionRepository,
    IMapper mapper) : ILinkService
{
    public async Task<TransactionLinkDto> AddTransactionAsync(Guid linkId, Guid transactionId)
    {
        var link = await linkRepository.EnsureExistsAsync(linkId);

        _ = await transactionRepository.EnsureExistsAsync(transactionId);

        // Проверяем что транзакция ещё не в этой связи
        if (link.Entries.Any(e => e.TransactionId == transactionId))
            throw new ArgumentException($"Transaction {transactionId} already in this link");

        link.Entries.Add(new TransactionLinkEntry
        {
            TransactionLinkId = linkId,
            TransactionId = transactionId
        });

        await linkRepository.UpdateAsync(link);
        await linkRepository.SaveChangesAsync();

        return mapper.Map<TransactionLinkDto>(link);
    }

    public async Task<TransactionLinkDto> CreateAsync(CreateTransactionLinkDto dto)
    {
        if (dto.TransactionIds.Count < 2)
            throw new ArgumentException("Связь должна содержать минимум две транзакции");

        foreach (var transactionId in dto.TransactionIds)
        {
            _ = await transactionRepository.EnsureExistsAsync(transactionId);
        }

        var link = new TransactionLink
        {
            Id = Guid.NewGuid(),
            Type = dto.Type,
            Entries = dto.TransactionIds.Select(transactionId => new TransactionLinkEntry
            {
                TransactionLinkId = default, // EF Core проставит сам
                TransactionId = transactionId
            }).ToList()
        };

        await linkRepository.AddAsync(link);
        await linkRepository.SaveChangesAsync();

        return mapper.Map<TransactionLinkDto>(link);
    }

    public async Task DeleteAsync(Guid id)
    {
        _ = await linkRepository.EnsureExistsAsync(id);

        await linkRepository.DeleteAsync(id);
        await linkRepository.SaveChangesAsync();
    }

    public async Task<TransactionLinkDto> GetByIdAsync(Guid id)
    {
        var link = await linkRepository.EnsureExistsWithEntriesAsync(id);
        return mapper.Map<TransactionLinkDto>(link);
    }

    public async Task RemoveTransactionAsync(Guid linkId, Guid transactionId)
    {
        var link = await linkRepository.EnsureExistsWithEntriesAsync(linkId);

        var entry = link.Entries.FirstOrDefault(e => e.TransactionId == transactionId)
            ?? throw new KeyNotFoundException($"Transaction {transactionId} not found in link {linkId}");

        // Нельзя оставить связь с одной транзакцией
        if (link.Entries.Count <= 2)
            throw new ArgumentException("Нельзя удалить транзакцию — в связи должно быть минимум две транзакции. Удалите всю связь.");

        link.Entries.Remove(entry);

        await linkRepository.UpdateAsync(link);
        await linkRepository.SaveChangesAsync();
    }
}
