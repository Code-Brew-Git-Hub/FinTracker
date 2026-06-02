using System.ComponentModel.DataAnnotations;

namespace FinTracker.Domain.Dtos.Transactions;

public class BulkUpdateDto
{
    [Required] public List<Guid> TransactionIds { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? ScopeId { get; set; }
    public bool DeleteScope { get; set; } = false;
    public string? Comment { get; set; }
    // null = не трогать теги, пустой список = очистить теги
    public List<Guid>? AddTagIds { get; set; }
    public List<Guid>? ReplaceTagIds { get; set; }
}
