namespace FinTracker.Domain.Dtos.Import;

public class ImportPresetMatchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public CsvParseOptionsDto ParseOptions { get; set; } = null!;
}
