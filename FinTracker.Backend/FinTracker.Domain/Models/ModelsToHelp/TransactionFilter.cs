using FinTracker.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinTracker.Domain.Models.ModelsToHelp;
/// <summary>
/// Фильтрация транзакций
/// </summary>
public class TransactionFilter
{
    /// <summary>
    /// По дате: с какого числа (включительно)
    /// </summary>
    public DateTime? DateFrom { get; set; }
    /// <summary>
    /// По дате: по какое число (включительно)
    /// </summary>
    public DateTime? DateTo { get; set; }
    /// <summary>
    /// По сумме: с какой суммы (включительно)
    /// </summary>
    public decimal? AmountMin { get; set; }
    /// <summary>
    /// По сумме: до какой суммы (включительно)
    /// </summary>
    public decimal? AmountMax { get; set; }
    /// <summary>
    /// По категории: id категории
    /// </summary>
    public Guid? CategoryId { get; set; }
    /// <summary>
    /// По типу: тип транзакции
    /// </summary>
    public TransactionType? Type { get; set; }
    /// <summary>
    /// По тегам: список id тегов
    /// </summary>
    public List<Guid>? TagIds { get; set; }
    /// <summary>
    /// По группе: id группы
    /// </summary>
    public Guid? ScopeId { get; set; }
    /// <summary>
    /// Быстрый поиск: описание, комментарий, имя категории, имена тегов
    /// </summary>
    [MaxLength(50, ErrorMessage = "Поисковый запрос не должен превышать 50 символов")]
    public string? Search { get; set; }
    /// <summary>
    /// Показать транзакции без группы
    /// </summary>
    public bool ExcludeScopes { get; set; }
    /// <summary>
    /// Номер страницы
    /// </summary>
    [Range(1, 1000, ErrorMessage = "Номер страницы должен быть больше 0 и меньше 1000")]
    public int Page { get; set; } = 1;
    /// <summary>
    /// Количество транзакий на странице
    /// </summary>
    [Range(1, 200, ErrorMessage = "Размер страницы должен быть от 1 до 200")]
    public int PageSize { get; set; } = 25;
}
