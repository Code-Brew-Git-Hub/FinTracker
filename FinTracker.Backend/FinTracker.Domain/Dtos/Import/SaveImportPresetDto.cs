namespace FinTracker.Domain.Dtos.Import;

public class SaveImportPresetDto
{
    public string Name { get; set; } = null!;
    public string[] MatchHeaders { get; set; } = [];
    public CsvParseOptionsDto ParseOptions { get; set; } = null!;
}
