using FinTracker.Domain.Dtos.Transactions;
using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Models;
using FinTracker.Domain.Models.ModelsToHelp;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.Data.Repositories;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task AddAsync(Transaction entity)
    {
        await context.Transactions
            .AddAsync(entity);
    }

    public async Task BulkUpdateAsync(BulkUpdateDto dto)
    {
        var ids = dto.TransactionIds;

        var transactions = await context.Transactions
        .Include(t => t.TransactionTags)
        .Where(t => ids.Contains(t.Id))
        .ToListAsync();

        foreach (var transaction in transactions)
        {
            if (dto.CategoryId != null)
                transaction.CategoryId = dto.CategoryId.Value;

            if (dto.ScopeId != null)
                transaction.ScopeId = dto.ScopeId.Value;

            if (dto.Comment != null)
                transaction.Comment = dto.Comment;

            if (dto.DeleteScope)
            {
                if (transaction.ScopeId == null)
                    throw new ArgumentException($"Transaction {transaction.Id} doesn't have scope");
                transaction.ScopeId = null;
            }

            if (dto.ReplaceTagIds != null)
            {
                //transaction.TransactionTags.Clear();
                transaction.TransactionTags = dto.ReplaceTagIds
                    .Select(tagId => new TransactionTag
                    {
                        TransactionId = transaction.Id,
                        TagId = tagId
                    }).ToList();
            }
            else if (dto.AddTagIds != null)
            {
                var existingTagIds = transaction.TransactionTags
                    .Select(tt => tt.TagId)
                    .ToHashSet();

                var newTags = dto.AddTagIds
                    .Where(tagId => !existingTagIds.Contains(tagId))
                    .Select(tagId => new TransactionTag
                    {
                        TransactionId = transaction.Id,
                        TagId = tagId
                    });

                foreach (var tag in newTags)
                    transaction.TransactionTags.Add(tag);
            }
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var transaction = await GetByIdAsync(id);
        if (transaction != null)
            context.Transactions
                .Remove(transaction);
    }

    public async Task<List<Transaction>> GetAllAsync()
    {
        return await context.Transactions
            .ToListAsync();
    }

    public async Task<Transaction?> GetByIdAsync(Guid id, bool includeDeleted)
    {
        var transaction = await context.Transactions
            .FindAsync(id);
        if (transaction == null)
            return null;
        return (includeDeleted || !transaction.IsDeleted) ? transaction : null;
    }

    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await GetByIdAsync(id, true);
    }

    public async Task<List<Transaction>> GetByScopeIdAsync(Guid scopeId)
    {
        return await context.Transactions
            .Where(t => t.ScopeId == scopeId)
            .ToListAsync();
    }

    public async Task<List<Transaction>> GetFilteredAsync(TransactionFilter filter, bool includeDeleted)
    {
        var query = context.Transactions
        .Include(t => t.Category)
        .Include(t => t.Scope)
        .Include(t => t.TransactionTags)
            .ThenInclude(tt => tt.Tag)
        .AsQueryable();

        if (filter.DateFrom != null)
            query = query.Where(t => t.DateUtc >= filter.DateFrom);

        if (filter.DateTo != null)
            query = query.Where(t => t.DateUtc <= filter.DateTo);

        if (filter.AmountMin != null)
            query = query.Where(t => t.Amount >= filter.AmountMin);

        if (filter.AmountMax != null)
            query = query.Where(t => t.Amount <= filter.AmountMax);

        if (filter.CategoryId != null)
            query = query.Where(t => t.CategoryId == filter.CategoryId);

        if (filter.Type != null)
            query = query.Where(t => t.Type == filter.Type);

        if (filter.ScopeId != null)
            query = query.Where(t => t.ScopeId == filter.ScopeId);

        if (filter.ExcludeScopes)
            query = query.Where(t => t.ScopeId == null);

        if (filter.TagIds != null && filter.TagIds.Any())
            query = query.Where(t => t.TransactionTags
                                      .Any(tt => filter.TagIds.Contains(tt.TagId)));

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var search = filter.Search;
            query = query.Where(t =>
                (t.Description != null && t.Description.Contains(search)) ||
                (t.Comment != null && t.Comment.Contains(search)) ||
                t.Category.Name.Contains(search) ||
                t.TransactionTags.Any(tt => tt.Tag.Name.Contains(search)));
        }

        if (!includeDeleted)
            query = query.Where(t => !t.IsDeleted);

        query = query
            .OrderBy(t => t.DateUtc)
            .ThenBy(t => t.Id);

        // Пагинация
        query = query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);

        return await query.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public void ClearChangeTracker()
    {
        context.ChangeTracker.Clear();
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        context.Transactions
            .Update(transaction);
    }
}
