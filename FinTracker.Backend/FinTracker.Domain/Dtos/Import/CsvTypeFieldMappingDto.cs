namespace FinTracker.Domain.Dtos.Import;

public class CsvTypeFieldMappingDto
{
    public CsvColumnFieldMappingDto Column { get; set; } = null!;
    public List<string>? IncomeValues { get; set; }
    public List<string>? ExpenseValues { get; set; }
}
