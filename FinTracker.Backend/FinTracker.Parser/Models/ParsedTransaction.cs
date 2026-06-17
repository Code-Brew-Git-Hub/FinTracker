namespace FinTracker.Parser.Models;

public class ParsedTransaction
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string CategoryName { get; set; }
    public string? Description { get; set; }
    public string? TypeRaw { get; set; }
}
