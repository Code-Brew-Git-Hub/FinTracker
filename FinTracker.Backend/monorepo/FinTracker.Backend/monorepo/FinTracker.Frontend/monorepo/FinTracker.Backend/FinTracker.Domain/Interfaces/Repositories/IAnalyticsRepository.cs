using FinTracker.Domain.Dtos.Analytics;
using FinTracker.Domain.Models;

namespace FinTracker.Domain.Interfaces.Repositories;

public interface IAnalyticsRepository
{
    Task<List<Transaction>> GetFilteredAsync(AnalyticsFilterDto filter);
}
