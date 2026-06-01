using FinTracker.Domain.Dtos.Scopes;

namespace FinTracker.Domain.Dtos.Analytics;

public class ScopeStatDto
{
    public ScopeDto Scope { get; set; }
    public decimal Total { get; set; }
    public int Count { get; set; }
}
