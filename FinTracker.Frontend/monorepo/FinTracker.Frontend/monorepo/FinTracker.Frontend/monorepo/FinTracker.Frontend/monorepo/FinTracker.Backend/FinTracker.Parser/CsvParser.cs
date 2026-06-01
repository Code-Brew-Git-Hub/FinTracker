using CsvHelper;
using CsvHelper.Configuration;
using FinTracker.Parser.Models;
using System.Globalization;

namespace FinTracker.Parser;

public class CsvParser
{
    private static readonly string TBankHeaders = "\"Дата операции\";\"Дата платежа\";\"Номер карты\";\"Статус\";\"Сумма операции\";\"Валюта операции\";\"Сумма платежа\";\"Валюта платежа\";\"Кэшбэк\";\"Категория\";\"MCC\";\"Описание\";\"Бонусы (включая кэшбэк)\";\"Округление на инвесткопилку\";\"Сумма операции с округлением\"";
    private static readonly string AlfaBankHeaders = "operationDate,transactionDate,accountName,accountNumber,cardName,cardNumber,merchant,amount,currency,status,category,mcc,type,comment,bonusValue,bonusTitle";

    private static readonly CsvConfiguration TBankConfig = new CsvConfiguration(CultureInfo.GetCultureInfo("ru-RU"))
    {
        HasHeaderRecord = true,
        IgnoreBlankLines = true,
        TrimOptions = TrimOptions.Trim,
        MissingFieldFound = null,
        Delimiter = ";"
    };

    private static readonly CsvConfiguration AlfaBankConfig = new CsvConfiguration(CultureInfo.GetCultureInfo("ru-RU"))
    {
        HasHeaderRecord = true,
        IgnoreBlankLines = true,
        TrimOptions = TrimOptions.Trim,
        MissingFieldFound = null,
        Delimiter = ","
    };

    public async Task<ParseResult> ParseCSV(StreamReader reader)
    {
        var baseStream = reader.BaseStream;
        bool canSeek = baseStream.CanSeek;
        long originalPosition = canSeek ? baseStream.Position : 0;

        var firstLine = await reader.ReadLineAsync();

        if (firstLine == TBankHeaders)
        {
            if (canSeek) baseStream.Position = originalPosition;
            reader.DiscardBufferedData();
            return await ParseWithMap<TBankCsvMap>(new CsvReader(reader, TBankConfig));
        }
        else if (firstLine == AlfaBankHeaders)
        {
            if (canSeek) baseStream.Position = originalPosition;
            reader.DiscardBufferedData();
            return await ParseWithMap<AlfaBankCsvMap>(new CsvReader(reader, AlfaBankConfig));
        }
        else
        {
            return new ParseResult() { Errors = new() { new ParseError() { Reason = "Не удалось распознать заголовки файла" } } };
        }

    }

    private async Task<ParseResult> ParseWithMap<TMap>(CsvReader csvReader)
        where TMap : ClassMap
    {
        csvReader.Context.RegisterClassMap<TMap>();

        var transactions = new List<ParsedTransaction>();
        var result = new ParseResult();
        var row = 1;

        while (await csvReader.ReadAsync())
        {
            row++;
            try
            {
                var transaction = csvReader.GetRecord<ParsedTransaction>();

                Normalize(transaction);

                var validationError = Validate(transaction, row);
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

    private void Normalize(ParsedTransaction t)
    {
        if (string.IsNullOrWhiteSpace(t.Description))
            t.Description = null;
    }

    private ParseError? Validate(ParsedTransaction t, int row)
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

    public sealed class TBankCsvMap : ClassMap<ParsedTransaction>
    {
        public TBankCsvMap()
        {
            Map(t => t.Date).Index(0);
            Map(t => t.Amount).Index(4);
            Map(t => t.Currency).Index(5);
            Map(t => t.CategoryName).Index(9);
            Map(t => t.Description).Index(11);
        }
    }

    public sealed class AlfaBankCsvMap : ClassMap<ParsedTransaction>
    {
        public AlfaBankCsvMap()
        {
            Map(t => t.Date).Index(0)
                .TypeConverterOption.Format("dd.MM.yyyy");
            Map(t => t.Amount).Index(7)
                .TypeConverterOption.NumberStyles(NumberStyles.Any)
                .TypeConverterOption.CultureInfo(CultureInfo.InvariantCulture);
            Map(t => t.Currency).Index(8);
            Map(t => t.CategoryName).Index(10);
        }
    }
}
