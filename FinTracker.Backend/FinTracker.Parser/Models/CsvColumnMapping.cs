namespace FinTracker.Parser.Models;

public class CsvColumnMapping
{
    public CsvColumnFieldMapping Date { get; set; } = null!;
    public CsvColumnFieldMapping Amount { get; set; } = null!;
    public CsvColumnFieldMapping Currency { get; set; } = null!;
    public CsvColumnFieldMapping CategoryName { get; set; } = null!;
    public CsvColumnFieldMapping? Description { get; set; }
    public CsvTypeFieldMapping? Type { get; set; }
}
