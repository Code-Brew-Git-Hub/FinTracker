using FinTracker.Domain.Dtos.Analytics;
using FinTracker.Domain.Models;

namespace FinTracker.Application.Abstractions.Repositories;

public interface IAnalyticsRepository
{
    Task<List<Transaction>> GetFilteredAsync(AnalyticsFilterDto filter);
}
