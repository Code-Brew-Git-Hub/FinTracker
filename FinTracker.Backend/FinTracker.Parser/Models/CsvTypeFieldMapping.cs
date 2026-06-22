namespace FinTracker.Parser.Models;

public class CsvTypeFieldMapping
{
    public CsvColumnFieldMapping Column { get; set; } = null!;
    public List<string> IncomeValues { get; set; } =
    [
        "income",
        "приход",
        "credit",
        "зачисление",
        "пополнение",
        "доход"
    ];
    public List<string> ExpenseValues { get; set; } =
    [
        "expense",
        "расход",
        "debit",
        "списание"
    ];
}
