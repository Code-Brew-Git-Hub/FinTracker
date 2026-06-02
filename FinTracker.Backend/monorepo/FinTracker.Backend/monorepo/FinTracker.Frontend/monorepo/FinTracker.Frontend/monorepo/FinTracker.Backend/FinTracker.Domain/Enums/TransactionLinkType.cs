namespace FinTracker.Domain.Enums;

/// <summary>
/// Тип связи транзакций: компенсация / перевод
/// </summary>
public enum TransactionLinkType
{
    /// <summary>
    /// Компенсация (дал в долг другу, он вернул через неделю)
    /// </summary>
    Compensation,
    /// <summary>
    /// Перевод (с одного счета на другой, или с одного банка на другой)
    /// </summary>
    Transfer
}
