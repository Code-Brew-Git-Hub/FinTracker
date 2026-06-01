using System.ComponentModel.DataAnnotations;

namespace FinTracker.Domain.Dtos.Transactions;

public class CreateTransactionDto
{
    [Required] public decimal Amount { get; set; }
    [Required] public string Currency { get; set; }
    [Required] public DateTime DateUtc { get; set; }
    public string? Description { get; set; }
    public string? Comment { get; set; }
    [Required] public Guid CategoryId { get; set; }
    public Guid? ScopeId { get; set; }
    public List<Guid> TagIds { get; set; } = [];
}
