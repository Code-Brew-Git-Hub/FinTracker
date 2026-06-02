namespace FinTracker.Parser.Models;

public class ParseResult
{
    public IEnumerable<ParsedTransaction> Transactions { get; set; } = [];
    public List<ParseError> Errors { get; set; } = [];
}
