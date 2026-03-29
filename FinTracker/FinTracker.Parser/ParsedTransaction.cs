using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace FinTracker.Parser
{
    public class ParsedTransaction
    {
        [Name("Дата операции")]
        [Format("dd.MM.yyyy HH:mm:ss")]
        public DateTime TransactionDate { get; set; }      // Дата операции   || 19.03.2026 14:37:08

        [Name("Дата платежа")]
        [Format("dd.MM.yyyy")]
        public DateOnly? PaymentDate { get; set; }         // Дата платежа    || 19.03.2026

        [Name("Номер карты")]
        [Length(5, 5)]
        public string? CardNumber { get; set; }            // Номер карты     || 6514

        [Name("Статус")]
        public string? Status { get; set; }                // Статус          || OK

        [Name("Сумма операции")]
        [NumberStyles(NumberStyles.Number)]
        public Decimal TransactionAmount { get; set; }     // Сумма операции  || 2530.02

        [Name("Валюта операции")]
        public string? TransactionCurrency { get; set; }   // Валюта операции || RUB

        [Name("Сумма платежа")]
        [NumberStyles(NumberStyles.Number)]
        public Decimal PaymentAmount { get; set; }         // Сумма платежа   || 2530.02

        [Name("Валюта платежа")]
        public string? PaymentCurrency { get; set; }       // Валюта платежа  || RUB

        [Name("Кэшбэк")]
        [NumberStyles(NumberStyles.Number)]
        public Decimal? CashBack { get; set; }             // Кэшбэк          || 12.3

        [Name("Категория")]
        public string? Category { get; set; }              // Категория       || Переводы

        [Name("MCC")]
        public int? MCC { get; set; }                      // MCC             || 5732

        [Name("Описание")]
        public string? Description { get; set; }           // Описание        || Между своими счетами

        [Name("Бонусы (включая кэшбэк)")]
        [NumberStyles(NumberStyles.Number)]
        public Decimal Bonuses { get; set; }               // Бонусы (включая кэшбэк) || 0.00   (в csv запятая, а не точка!)

        [Name("Округление на инвесткопилку")]
        [NumberStyles(NumberStyles.Number)]
        public Decimal RoundToCoinBox { get; set; }        // Округление на инвесткопилку || 0.00   (в csv запятая, а не точка!)

        [Name("Сумма операции с округлением")]
        [NumberStyles(NumberStyles.Number)]
        public Decimal TotalAmount { get; set; }           // Сумма операции с округлением || 2530,02


        public override string ToString()
        {
            var str = new StringBuilder();

            str.AppendLine($"Дата операции:{TransactionDate}");
            str.AppendLine($"Дата платежа:{PaymentDate}");
            str.AppendLine($"Номер карты:{CardNumber}");
            str.AppendLine($"Статус:{Status}");
            str.AppendLine($"Сумма операции:{TransactionAmount}");
            str.AppendLine($"Валюта операции:{TransactionCurrency}");
            str.AppendLine($"Сумма платежа:{PaymentAmount}");
            str.AppendLine($"Валюта платежа:{PaymentCurrency}");
            str.AppendLine($"Кэшбэк:{CashBack}");
            str.AppendLine($"Категория:{Category}");
            str.AppendLine($"MCC:{MCC}");
            str.AppendLine($"Описание:{Description}");
            str.AppendLine($"Бонусы (включая кэшбэк):{Bonuses}");
            str.AppendLine($"Округление на инвесткопилку:{RoundToCoinBox}");
            str.AppendLine($"Сумма операции с округлением:{TotalAmount}");

            return str.ToString();
        }
    }
}
