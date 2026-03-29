using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using FinTracker.Domain;


namespace FinTracker.Parser
{
    public class CsvParser
    {
        public static IEnumerable<Transaction> ParseCSV(StreamReader reader)
        {
            var config = new CsvConfiguration(CultureInfo.GetCultureInfo("ru-RU"))
            {
                HasHeaderRecord = true,
                IgnoreBlankLines = true,  // Скипать пустые строки | Наверное не нужно здесь
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
            };

            using var csvReader = new CsvReader(reader, config);
            var parsedTransactions = csvReader.GetRecords<ParsedTransaction>().ToList();

            return (IEnumerable<Transaction>)parsedTransactions;
        }
    }
}
