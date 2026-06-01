using FinTracker.Domain.Dtos.Tags;

namespace FinTracker.Domain.Dtos.Analytics;

public class TagStatDto
{
    public TagDto Tag { get; set; }
    public decimal Total { get; set; }
    public int Count { get; set; }
}
