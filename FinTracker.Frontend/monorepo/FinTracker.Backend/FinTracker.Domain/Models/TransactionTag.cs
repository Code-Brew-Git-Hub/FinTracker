
namespace FinTracker.Domain.Models;

/// <summary>
/// Свзяь Many-to-many: Transaction <-> Tag
/// (Нужно для бд)
/// </summary>
public class TransactionTag
{
    /// <summary>
    /// Id транзакции
    /// </summary>
    public Guid TransactionId { get; set; }
    /// <summary>
    /// Транзакция
    /// </summary>
    public Transaction Transaction { get; set; }

    /// <summary>
    /// Id тега
    /// </summary>
    public Guid TagId { get; set; }
    /// <summary>
    /// Тег
    /// </summary>
    public Tag Tag { get; set; }
}