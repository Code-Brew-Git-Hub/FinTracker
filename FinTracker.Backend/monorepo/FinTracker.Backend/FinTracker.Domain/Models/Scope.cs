
namespace FinTracker.Domain.Models;
/// <summary>
/// Группа
/// </summary>
public class Scope
{
    /// <summary>
    /// Id группы
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Название группы
    /// </summary>
    public string Name { get; set; }
    //public string? Description { get; set; }

    /// <summary>
    /// Коллекция транзакций, относящихся к этой группе
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = [];
}