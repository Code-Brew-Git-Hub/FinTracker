namespace FinTracker.Domain.Models;

public class TransactionLinkEntry
{
    public Guid TransactionLinkId { get; set; }
    public TransactionLink TransactionLink { get; set; }
    public Guid TransactionId { get; set; }
    public Transaction Transaction { get; set; }
}
