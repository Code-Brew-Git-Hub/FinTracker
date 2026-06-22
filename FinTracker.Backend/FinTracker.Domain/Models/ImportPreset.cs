namespace FinTracker.Domain.Models;

public class ImportPreset
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string ParseOptionsJson { get; set; } = null!;
    public string MatchHeadersJson { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
