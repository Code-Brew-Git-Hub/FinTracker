namespace FinTracker.Domain.Models;
/// <summary>
/// Категория
/// </summary>
public class Category
{
    /// <summary>
    /// Id категории
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Название категории
    /// </summary>
    public string Name { get; set; }

    // Navigation

    /// <summary>
    /// Коллекция транзакций, к которым относится категория
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = [];
    /*
    public ICollection<Position> Positions { get; set; } = [];
    */
}
