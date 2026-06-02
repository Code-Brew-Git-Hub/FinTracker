using FinTracker.Domain.Enums;

namespace FinTracker.Domain.Dtos.TransactionLinks;

public class CreateTransactionLinkDto
{
    public TransactionLinkType Type { get; set; }
    public List<Guid> TransactionIds { get; set; }
}
