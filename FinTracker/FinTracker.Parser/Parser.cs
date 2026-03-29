using FinTracker.Domain;

namespace FinTracker.Parser
{
    public class Parser
    {
        private static readonly Dictionary<string, Func<StreamReader, IEnumerable<Transaction>>> ExtensionToParseFunction = new()
        {
            { ".csv", CsvParser.ParseCSV },
            { ".xlsx", XlsxParser.ParseXLSX },
            { ".xls", XlsxParser.ParseXLSX },
            { ".pdf", PdfParser.ParsePDF }
        };

        public static IEnumerable<Transaction> Parse(StreamReader reader, string filename)
        {
            var sucsess = TryParse(reader, filename, out var transactions);

            if (sucsess)
                return transactions;

            throw new ArgumentException("Введен неправильный файл");
        }

        public static bool TryParse(StreamReader reader, string filename, out IEnumerable<Transaction> transactions)
        {
            transactions = null;
            var extension = Path.GetExtension(filename);

            if (!ExtensionToParseFunction.TryGetValue(extension, out Func<StreamReader, IEnumerable<Transaction>>? value))
                return false;

            transactions = value(reader);
            return true;
        }
    }
}
