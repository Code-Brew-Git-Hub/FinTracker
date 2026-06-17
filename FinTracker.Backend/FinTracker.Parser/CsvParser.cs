using CsvHelper;
using CsvHelper.Configuration;
using FinTracker.Parser.Models;
using System.Globalization;

namespace FinTracker.Parser;

public class CsvParser
{
    private static readonly string[] DelimiterCandidates = [";", ",", "\t"];

    public async Task<CsvFileStructure> ReadFileStructureAsync(StreamReader reader)
    {
        var (buffered, dispose) = await PrepareReaderAsync(reader);
        try
        {
            var firstLine = await buffered.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(firstLine))
                throw new ArgumentException("Файл пустой или не содержит заголовков");

            var delimiter = DetectDelimiter(firstLine);
            var config = CreateConfiguration(delimiter, "ru-RU", hasHeaderRecord: true);
            var headers = await ReadHeadersAsync(buffered, config);

            return new CsvFileStructure
            {
                Headers = headers,
                DetectedDelimiter = delimiter
            };
        }
        finally
        {
            if (dispose)
                buffered.Dispose();
        }
    }

    public async Task<ParseResult> ParseAsync(StreamReader reader, CsvParseOptions options)
    {
        var (buffered, dispose) = await PrepareReaderAsync(reader);
        try
        {
            var config = CreateConfiguration(options.Delimiter, options.Culture, options.HasHeaderRecord);
            string[] headers = [];

            if (options.HasHeaderRecord)
            {
                headers = await ReadHeadersAsync(buffered, config);
                CsvMappingValidator.Validate(headers, options.ColumnMapping);
            }

            ResetReader(buffered);

            using var csvReader = new CsvReader(buffered, config);
            csvReader.Context.RegisterClassMap(new DynamicCsvMap(options.ColumnMapping, options));

            if (options.HasHeaderRecord)
            {
                await csvReader.ReadAsync();
                csvReader.ReadHeader();
            }

            return await ParseRowsAsync(csvReader, options, dataStartRow: options.HasHeaderRecord ? 2 : 1);
        }
        finally
        {
            if (dispose)
                buffered.Dispose();
        }
    }

    private async Task<ParseResult> ParseRowsAsync(CsvReader csvReader, CsvParseOptions options, int dataStartRow)
    {
        var transactions = new List<ParsedTransaction>();
        var result = new ParseResult();
        var row = dataStartRow - 1;

        while (await csvReader.ReadAsync())
        {
            row++;
            try
            {
                var transaction = csvReader.GetRecord<ParsedTransaction>();

                var normalizationError = Normalize(transaction, options);
                if (normalizationError != null)
                {
                    result.Errors.Add(new ParseError { Row = row, Reason = normalizationError });
                    continue;
                }

                var validationError = Validate(transaction, row, options);
                if (validationError != null)
                {
                    result.Errors.Add(validationError);
                    continue;
                }

                transactions.Add(transaction);
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ParseError { Row = row, Reason = ex.Message });
            }
        }

        result.Transactions = transactions;
        return result;
    }

    private static string? Normalize(ParsedTransaction t, CsvParseOptions options)
    {
        if (string.IsNullOrWhiteSpace(t.Description))
            t.Description = null;

        var typeMapping = options.ColumnMapping.Type;
        if (typeMapping == null)
            return null;

        if (string.IsNullOrWhiteSpace(t.TypeRaw))
            return "Тип транзакции не указан";

        var raw = t.TypeRaw.Trim();
        if (MatchesAny(raw, typeMapping.IncomeValues))
        {
            t.Amount = Math.Abs(t.Amount);
            return null;
        }

        if (MatchesAny(raw, typeMapping.ExpenseValues))
        {
            t.Amount = -Math.Abs(t.Amount);
            return null;
        }

        return $"Неизвестный тип транзакции: {t.TypeRaw}";
    }

    private static bool MatchesAny(string raw, IEnumerable<string> values) =>
        values.Any(v => raw.Equals(v, StringComparison.OrdinalIgnoreCase));

    private ParseError? Validate(ParsedTransaction t, int row, CsvParseOptions options)
    {
        if (string.IsNullOrWhiteSpace(t.CategoryName))
            return new ParseError { Row = row, Reason = "Категория не указана" };
        if (string.IsNullOrWhiteSpace(t.Currency))
            return new ParseError { Row = row, Reason = "Валюта не указана" };

        if (t.Amount == 0)
            return new ParseError { Row = row, Reason = "Сумма равна нулю" };

        if (t.Date == default)
            return new ParseError { Row = row, Reason = "Дата не указана" };

        return null;
    }

    private static string DetectDelimiter(string headerLine)
    {
        var best = ";";
        var maxColumns = 0;

        foreach (var delimiter in DelimiterCandidates)
        {
            var count = headerLine.Split(delimiter).Length;
            if (count > maxColumns)
            {
                maxColumns = count;
                best = delimiter;
            }
        }

        return best;
    }

    private static async Task<string[]> ReadHeadersAsync(StreamReader reader, CsvConfiguration config)
    {
        ResetReader(reader);

        using var csv = new CsvReader(reader, config);
        await csv.ReadAsync();
        csv.ReadHeader();

        return NormalizeHeaders(csv.HeaderRecord ?? []);
    }

    private static string[] NormalizeHeaders(string[] headers)
    {
        if (headers.Length == 0)
            return headers;

        var normalized = new string[headers.Length];
        for (var i = 0; i < headers.Length; i++)
        {
            var value = headers[i]?.Trim() ?? string.Empty;
            if (i == 0)
                value = value.TrimStart('\ufeff');

            normalized[i] = value;
        }

        return normalized;
    }

    private static CsvConfiguration CreateConfiguration(string delimiter, string culture, bool hasHeaderRecord) =>
        new(CultureInfo.GetCultureInfo(culture))
        {
            HasHeaderRecord = hasHeaderRecord,
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null,
            Delimiter = delimiter
        };

    private static void ResetReader(StreamReader reader)
    {
        reader.BaseStream.Position = 0;
        reader.DiscardBufferedData();
    }

    private static async Task<(StreamReader Reader, bool ShouldDispose)> PrepareReaderAsync(StreamReader reader)
    {
        if (reader.BaseStream.CanSeek)
        {
            ResetReader(reader);
            return (reader, false);
        }

        var memoryStream = new MemoryStream();
        await reader.BaseStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        return (new StreamReader(memoryStream), true);
    }
}
