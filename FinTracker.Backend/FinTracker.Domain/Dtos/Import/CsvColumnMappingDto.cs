namespace FinTracker.Domain.Dtos.Import;

public class CsvColumnMappingDto
{
    public CsvColumnFieldMappingDto Date { get; set; } = null!;
    public CsvColumnFieldMappingDto Amount { get; set; } = null!;
    public CsvColumnFieldMappingDto Currency { get; set; } = null!;
    public CsvColumnFieldMappingDto CategoryName { get; set; } = null!;
    public CsvColumnFieldMappingDto? Description { get; set; }
    public CsvTypeFieldMappingDto? Type { get; set; }
}
