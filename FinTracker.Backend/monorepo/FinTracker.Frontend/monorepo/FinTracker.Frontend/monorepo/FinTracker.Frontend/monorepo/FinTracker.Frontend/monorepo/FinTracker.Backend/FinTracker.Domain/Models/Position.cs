namespace FinTracker.Domain.Models;
/// <summary>
/// Позиция транзакции
/// </summary>
public class Position
{
    /// <summary>
    /// Id позиции
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Название позиции
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Сумма позиции
    /// </summary>
    public decimal Amount { get; set; }
    /// <summary>
    /// Id транзакции, в которой находится позиция
    /// </summary>
    public Guid TransactionId { get; set; }
    /// <summary>
    /// Транзакция, в которой находится позиция
    /// </summary>
    public Transaction Transaction { get; set; }
    /// <summary>
    /// Id категории, к которой относится позиция
    /// </summary>
    public Guid? CategoryId { get; set; }
    /// <summary>
    /// Категория, к которой относится позиция
    /// </summary>
    public Category? Category { get; set; }
}