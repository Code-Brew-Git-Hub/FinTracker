namespace FinTracker.Domain.Dtos.Transactions;

public class TransactionPreviewDto
{
    public DateTime DateUtc { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; }
}
