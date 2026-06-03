using FinTracker.Domain.Enums;

namespace FinTracker.Domain.Models;
/// <summary>
/// Транзакция
/// </summary>
public class Transaction
{
    /// <summary>
    /// Id транзакции
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Сумма транзакции
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// Валюта транзакции
    /// </summary>
    public string Currency { get; set; } = string.Empty;
    /// <summary>
    /// Дата платежа (UTC)
    /// </summary>
    public DateTime DateUtc { get; set; }
    /// <summary>
    /// Описание
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// Комментарий пользователя
    /// </summary>
    public string? Comment { get; set; }
    /// <summary>
    /// Тип транзакции (Доход / Расход)
    /// </summary>
    public TransactionType Type { get; set; }
    /// <summary>
    /// Признак soft delete
    /// </summary>
    public bool IsDeleted { get; set; }

    // Foreign Keys

    /// <summary>
    /// Id категории
    /// </summary>
    public Guid CategoryId { get; set; }
    /// <summary>
    /// Id группы
    /// </summary>
    public Guid? ScopeId { get; set; }

    // Navigation

    /// <summary>
    /// Категория
    /// </summary>
    public Category Category { get; set; }
    /// <summary>
    /// Группа
    /// </summary>
    public Scope? Scope { get; set; }

    /// <summary>
    /// Теги
    /// </summary>
    public ICollection<TransactionTag> TransactionTags { get; set; } = [];
    /// <summary>
    /// Позиции транзакции
    /// </summary>
    public ICollection<Position> Positions { get; set; } = [];  // (хлеб, колбаса, вода)
    /// <summary>
    /// Связи транзакции
    /// </summary>
    public ICollection<TransactionLinkEntry> LinkEntries { get; set; } = [];  // (коменсация / перевод)
}