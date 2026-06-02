using FinTracker.Domain.Dtos.Transactions;

namespace FinTracker.Domain.Dtos.Import;

public class ImportResultDto
{
    public int Total { get; set; }
    public int Imported { get; set; }
    public List<ImportErrorDto> Errors { get; set; } = [];
    public List<CategoryImportStatDto> Categories { get; set; } = [];
    public DateRangeDto? Period { get; set; }
    public int IncomeCount { get; set; }
    public int ExpenseCount { get; set; }
    public List<TransactionPreviewDto> Preview { get; set; } = [];
}
