namespace FinTracker.Domain.Dtos.Import;

public class CsvParseOptionsDto
{
    public string Delimiter { get; set; } = ";";
    public string Culture { get; set; } = "ru-RU";
    public string? DateFormat { get; set; }
    public string? NumberCulture { get; set; }
    public bool HasHeaderRecord { get; set; } = true;
    public CsvColumnMappingDto ColumnMapping { get; set; } = null!;
}
