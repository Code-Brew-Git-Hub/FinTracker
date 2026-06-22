namespace FinTracker.Domain.Dtos.Import;

public class ImportPresetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string[] MatchHeaders { get; set; } = [];
    public CsvParseOptionsDto ParseOptions { get; set; } = null!;
}
