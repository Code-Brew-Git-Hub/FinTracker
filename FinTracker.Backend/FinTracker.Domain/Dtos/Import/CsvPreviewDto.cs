namespace FinTracker.Domain.Dtos.Import;

public class CsvPreviewDto
{
    public string[] Headers { get; set; } = [];
    public string DetectedDelimiter { get; set; } = ";";
    public Guid? MatchedPresetId { get; set; }
    public string? MatchedPresetName { get; set; }
    public CsvParseOptionsDto? SuggestedParseOptions { get; set; }
    public List<ImportPresetListItemDto> Presets { get; set; } = [];
}
