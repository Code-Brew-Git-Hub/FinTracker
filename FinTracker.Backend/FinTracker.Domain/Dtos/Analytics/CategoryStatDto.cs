using FinTracker.Domain.Dtos.Categories;

namespace FinTracker.Domain.Dtos.Analytics;

public class CategoryStatDto
{
    public CategoryDto Category { get; set; }
    public decimal Total { get; set; }
    public int Count { get; set; }
    public decimal Percent { get; set; }
}
