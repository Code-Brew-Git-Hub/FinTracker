namespace FinTracker.Parser.Models;

public class CsvParseOptions
{
    public string Delimiter { get; set; } = ";";
    public string Culture { get; set; } = "ru-RU";
    public string? DateFormat { get; set; }
    public string? NumberCulture { get; set; }
    public bool HasHeaderRecord { get; set; } = true;
    public CsvColumnMapping ColumnMapping { get; set; } = null!;
}
