using FinTracker.Domain.Enums;

namespace FinTracker.Domain.Models;
/// <summary>
/// Связь транзакции
/// </summary>
public class TransactionLink
{
    /// <summary>
    /// Id связи
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Тип связи (коменсация / перевод)
    /// </summary>
    public TransactionLinkType Type { get; set; }
    /// <summary>
    /// Коллекция связей с другими транзакциями
    /// </summary>
    public ICollection<TransactionLinkEntry> Entries { get; set; } = [];
}