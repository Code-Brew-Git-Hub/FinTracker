using FinTracker.Domain.Dtos.Analytics;
using FinTracker.Domain.Enums;

namespace FinTracker.Application.Abstractions.Services;

public interface IAnalyticsService
{
    Task<AnalyticsSummaryDto> GetSummaryAsync(AnalyticsFilterDto filter);
    Task<List<CategoryStatDto>> GetByCategoryAsync(AnalyticsFilterDto filter);
    Task<List<TagStatDto>> GetByTagAsync(AnalyticsFilterDto filter);
    Task<IEnumerable<ScopeStatDto>> GetByScopeAsync(AnalyticsFilterDto filter);
    Task<List<TimeStatDto>> GetByTimeAsync(AnalyticsFilterDto filter, TimeGrouping grouping);
}
