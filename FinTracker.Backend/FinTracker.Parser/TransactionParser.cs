using FinTracker.Parser.Models;

namespace FinTracker.Parser;

public class TransactionParser
{
    private readonly CsvParser _csvParser;

    public TransactionParser(CsvParser csvParser)
    {
        _csvParser = csvParser;
    }
    public async Task<ParseResult> Parse(StreamReader reader, string filename)
    {
        var extension = Path.GetExtension(filename).ToLowerInvariant();

        return extension switch
        {
            ".csv" => await _csvParser.ParseCSV(reader),
            _ => new ParseResult() { Errors = new() { new() { Reason = "Не поддерживаемый тип файла" } } }
        };
    }
}
