namespace FinTracker.Domain.Models;
/// <summary>
/// Тег транзакции
/// </summary>
public class Tag
{
    /// <summary>
    /// Id тега
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Название тега
    /// </summary>
    public string Name { get; set; }
}