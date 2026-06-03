using FinTracker.Domain.Dtos.Transactions;
using FinTracker.Domain.Enums;
using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Models;
using FinTracker.Domain.Models.ModelsToHelp;

namespace FinTracker.Data.Services;

public class TransactionService(ITransactionRepository transactionRepository) : ITransactionService
{
    public async Task BulkUpdateAsync(BulkUpdateDto dto)
    {
        await transactionRepository.BulkUpdateAsync(dto);
        await transactionRepository.SaveChangesAsync();
    }

    public async Task<Transaction> CreateAsync(CreateTransactionDto dto)
    {
        var newId = Guid.NewGuid();
        var newTransaction = new Transaction()
        {
            Id = newId,
            Amount = dto.Amount,
            Currency = dto.Currency,
            DateUtc = dto.DateUtc.ToUniversalTime(),
            Description = dto.Description,
            Comment = dto.Comment,
            Type = dto.Amount < 0 ? TransactionType.Expense : TransactionType.Income,
            IsDeleted = false,
            CategoryId = dto.CategoryId,
            ScopeId = dto.ScopeId,
            TransactionTags = dto.TagIds.Select(tagId => new TransactionTag
            {
                TransactionId = newId,
                TagId = tagId
            }).ToList()
        };

        await transactionRepository.AddAsync(newTransaction);
        await transactionRepository.SaveChangesAsync();
        return newTransaction;
    }

    public async Task DeleteAsync(Guid id)
    {
        await transactionRepository.DeleteAsync(id);
        await transactionRepository.SaveChangesAsync();
    }

    public async Task<Transaction> GetByIdAsync(Guid id, bool includeDeleted)
    {
        return await transactionRepository.EnsureExistsAsync(id, includeDeleted);
    }

    public async Task<Transaction> UpdateAsync(Guid id, UpdateTransactionDto dto)
    {
        var transaction = await transactionRepository.EnsureExistsAsync(id);

        if (dto.Amount != null)
        {
            transaction.Amount = (decimal)dto.Amount;
            transaction.Type = (decimal)dto.Amount < 0 ? TransactionType.Expense : TransactionType.Income;
        }
        if (dto.Currency != null)
            transaction.Currency = dto.Currency;
        if (dto.DateUtc != null)
            transaction.DateUtc = (DateTime)dto.DateUtc;
        if (dto.Description != null)
            transaction.Description = dto.Description;
        if (dto.Comment != null)
            transaction.Comment = dto.Comment;
        if (dto.CategoryId != null)
            transaction.CategoryId = dto.CategoryId.Value;
        if (dto.ScopeId != null)
            transaction.ScopeId = dto.ScopeId;
        if (dto.DeleteScope)
        {
            if (transaction.ScopeId == null)
                throw new ArgumentException($"Transaction {transaction.Id} doesn't have scope");
            transaction.ScopeId = null;
        }
        if (dto.TagIds != null && dto.TagIds.Any())
        {
            //transaction.TransactionTags.Clear();
            transaction.TransactionTags = dto.TagIds
                .Select(tagId => new TransactionTag
                {
                    TransactionId = transaction.Id,
                    TagId = tagId
                }).ToList();
        }

        await transactionRepository.UpdateAsync(transaction);
        await transactionRepository.SaveChangesAsync();

        return transaction;
    }

    public async Task<List<Transaction>> GetFilteredAsync(TransactionFilter filter, bool includeDeleted)
    {
        return await transactionRepository.GetFilteredAsync(filter, includeDeleted);
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var transaction = await transactionRepository.EnsureExistsAsync(id);
        transaction.IsDeleted = true;

        await transactionRepository.UpdateAsync(transaction);
        await transactionRepository.SaveChangesAsync();
    }
}
