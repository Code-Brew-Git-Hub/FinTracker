using FinTracker.Domain.Enums;

namespace FinTracker.Domain.Dtos.Analytics;

public class AnalyticsFilterDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    // Фильтрация
    public decimal? AmountMin { get; set; }
    public decimal? AmountMax { get; set; }
    public Guid? CategoryId { get; set; }
    public List<Guid>? TagIds { get; set; }
    public TransactionType? Type { get; set; }
    public Guid? ScopeId { get; set; }

    // Исключения
    public List<Guid>? ExcludeScopeIds { get; set; }
    public bool ExcludeTransfers { get; set; } = true;
    public bool ExcludeCompensations { get; set; } = false;
}
