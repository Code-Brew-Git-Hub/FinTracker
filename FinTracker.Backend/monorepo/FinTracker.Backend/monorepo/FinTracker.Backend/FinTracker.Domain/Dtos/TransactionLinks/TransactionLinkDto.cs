using FinTracker.Domain.Dtos.Transactions;

namespace FinTracker.Domain.Dtos.TransactionLinks;

public class TransactionLinkDto
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public List<TransactionDto> Transactions { get; set; }
}
