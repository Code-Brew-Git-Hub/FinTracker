using FinTracker.Parser.Models;

namespace FinTracker.Parser;

public class TransactionParser(CsvParser csvParser)
{
    public async Task<CsvFileStructure> ReadFileStructureAsync(StreamReader reader, string filename)
    {
        EnsureCsvExtension(filename);
        return await csvParser.ReadFileStructureAsync(reader);
    }

    public async Task<ParseResult> ParseAsync(StreamReader reader, string filename, CsvParseOptions options)
    {
        EnsureCsvExtension(filename);
        return await csvParser.ParseAsync(reader, options);
    }

    private static void EnsureCsvExtension(string filename)
    {
        var extension = Path.GetExtension(filename).ToLowerInvariant();
        if (extension != ".csv")
            throw new ArgumentException("Поддерживается только формат CSV");
    }
}
