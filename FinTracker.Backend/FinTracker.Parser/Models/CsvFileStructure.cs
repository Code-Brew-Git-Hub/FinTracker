namespace FinTracker.Parser.Models;

public class CsvFileStructure
{
    public string[] Headers { get; set; } = [];
    public string DetectedDelimiter { get; set; } = ";";
}
