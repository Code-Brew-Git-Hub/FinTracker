using FinTracker.Application.Abstractions.Repositories;
using FinTracker.Domain.Dtos.Analytics;
using FinTracker.Domain.Enums;
using FinTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.Data.Repositories;

public class AnalyticsRepository(AppDbContext context) : IAnalyticsRepository
{
    public async Task<List<Transaction>> GetFilteredAsync(AnalyticsFilterDto filter)
    {
        var query = context.Transactions
            .Include(t => t.TransactionTags).ThenInclude(tt => tt.Tag)
            .Include(t => t.Category)
            .Include(t => t.Scope)
            .Include(t => t.LinkEntries).ThenInclude(le => le.TransactionLink)
            .Where(t => !t.IsDeleted)
            .AsQueryable();

        if (filter.DateFrom != null)
            query = query.Where(t => t.DateUtc >= filter.DateFrom);

        if (filter.DateTo != null)
            query = query.Where(t => t.DateUtc <= filter.DateTo);

        if (filter.AmountMin != null)
            query = query.Where(t => t.Amount >= filter.AmountMin || t.Amount <= -filter.AmountMin);

        if (filter.AmountMax != null)
            query = query.Where(t => t.Amount <= filter.AmountMax && t.Amount >= -filter.AmountMax);

        if (filter.CategoryId != null)
            query = query.Where(t => t.CategoryId == filter.CategoryId);

        if (filter.TagIds != null && filter.TagIds.Any())
            query = query.Where(t => t.TransactionTags
                .Any(tt => filter.TagIds.Contains(tt.TagId)));

        if (filter.Type != null)
            query = query.Where(t => t.Type == filter.Type);

        if (filter.ScopeId != null)
            query = query.Where(t => t.ScopeId == filter.ScopeId);

        if (filter.ExcludeScopeIds != null && filter.ExcludeScopeIds.Any())
            query = query.Where(t => t.ScopeId == null ||
                                     !filter.ExcludeScopeIds.Contains(t.ScopeId.Value));

        if (filter.ExcludeTransfers)
            query = query.Where(t => t.LinkEntries
                .All(le => le.TransactionLink.Type != TransactionLinkType.Transfer));

        if (filter.ExcludeCompensations)
            query = query.Where(t => t.LinkEntries
                .All(le => le.TransactionLink.Type != TransactionLinkType.Compensation));

        return await query.ToListAsync();
    }
}
