using System.Globalization;
using FinTracker.Application.Abstractions.Repositories;
using FinTracker.Application.Abstractions.Services;
using FinTracker.Domain.Dtos.Analytics;
using FinTracker.Domain.Dtos.Categories;
using FinTracker.Domain.Dtos.Scopes;
using FinTracker.Domain.Dtos.Tags;
using FinTracker.Domain.Enums;

namespace FinTracker.Application.Services;

public class AnalyticsService(IAnalyticsRepository analyticsRepository) : IAnalyticsService
{
    public async Task<List<CategoryStatDto>> GetByCategoryAsync(AnalyticsFilterDto filter)
    {
        var transactions = await analyticsRepository.GetFilteredAsync(filter);

        var totalExpense = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => Math.Abs(t.Amount));

        return transactions
            .GroupBy(t => t.Category)
            .Select(g => new CategoryStatDto
            {
                Category = new CategoryDto() { Id = g.Key.Id, Name = g.Key.Name },
                Total = g.Sum(t => Math.Abs(t.Amount)),
                Count = g.Count(),
                // Процент считается только если есть расходы, чтобы избежать деления на ноль
                Percent = totalExpense > 0
                    // Сумма расходов по категории / общая сумма расходов * 100, округляем до 2 знаков
                    ? Math.Round(g.Sum(t => Math.Abs(t.Amount)) / totalExpense * 100, 2)
                    // Сумма расходов по категории / общая сумма расходов * 100, округляем до 2 знаков
                    : 0
            })
            .OrderByDescending(x => x.Total)
            .ToList();
    }

    public async Task<List<TagStatDto>> GetByTagAsync(AnalyticsFilterDto filter)
    {
        var transactions = await analyticsRepository.GetFilteredAsync(filter);

        return transactions
            .Where(t => t.TransactionTags.Any())
            .SelectMany(t => t.TransactionTags.Select(tt => new { tt.Tag, t.Amount }))
            .GroupBy(x => x.Tag)
            .Select(g => new TagStatDto
            {
                Tag = new TagDto() { Id = g.Key.Id, Name = g.Key.Name },
                Total = g.Sum(x => Math.Abs(x.Amount)),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Total)
            .ToList();
    }

    public async Task<IEnumerable<ScopeStatDto>> GetByScopeAsync(AnalyticsFilterDto filter)
    {
        var transactions = await analyticsRepository.GetFilteredAsync(filter);

        return transactions
            .Where(t => t.ScopeId != null)
            .GroupBy(t => t.Scope!)
            .Select(g => new ScopeStatDto
            {
                Scope = new ScopeDto() { Id = g.Key.Id, Name = g.Key.Name },
                Total = g.Sum(t => Math.Abs(t.Amount)),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Total)
            .ToList();
    }

    public async Task<List<TimeStatDto>> GetByTimeAsync(AnalyticsFilterDto filter, TimeGrouping grouping)
    {
        var transactions = await analyticsRepository.GetFilteredAsync(filter);

        return transactions
            .GroupBy(t => grouping switch
            {
                TimeGrouping.Day => t.DateUtc.ToString("yyyy-MM-dd"),
                TimeGrouping.Week => $"{t.DateUtc.Year}-W{ISOWeek.GetWeekOfYear(t.DateUtc):D2}",
                TimeGrouping.Month => t.DateUtc.ToString("yyyy-MM"),
                _ => t.DateUtc.ToString("yyyy-MM")
            })
            .Select(g => new TimeStatDto
            {
                Period = g.Key,
                TotalIncome = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                TotalExpense = g.Where(t => t.Type == TransactionType.Expense).Sum(t => Math.Abs(t.Amount)),
                Balance = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount)
                             - g.Where(t => t.Type == TransactionType.Expense).Sum(t => Math.Abs(t.Amount))
            })
            .OrderBy(x => x.Period)
            .ToList();
    }

    public async Task<AnalyticsSummaryDto> GetSummaryAsync(AnalyticsFilterDto filter)
    {
        var transactions = await analyticsRepository.GetFilteredAsync(filter);

        var income = transactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var expense = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => Math.Abs(t.Amount));

        return new AnalyticsSummaryDto
        {
            TotalIncome = income,
            TotalExpense = expense,
            Balance = income - expense
        };
    }
}
