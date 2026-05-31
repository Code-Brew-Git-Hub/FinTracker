namespace FinTracker.Domain.Dtos.Analytics;

public class TimeStatDto
{
    public string Period { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance { get; set; }
}
