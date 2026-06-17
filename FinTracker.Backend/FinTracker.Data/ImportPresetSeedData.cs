using FinTracker.Domain.Dtos.Import;
using FinTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.Data;

public static class ImportPresetSeedData
{
    public static readonly Guid TBankPresetId = Guid.Parse("a1b2c3d4-e5f6-4789-a012-3456789abcde");
    public static readonly Guid AlfaBankPresetId = Guid.Parse("b2c3d4e5-f6a7-4890-b123-456789abcdef");

    private static readonly string[] TBankHeaders =
    [
        "Дата операции",
        "Дата платежа",
        "Номер карты",
        "Статус",
        "Сумма операции",
        "Валюта операции",
        "Сумма платежа",
        "Валюта платежа",
        "Кэшбэк",
        "Категория",
        "MCC",
        "Описание",
        "Бонусы (включая кэшбэк)",
        "Округление на инвесткопилку",
        "Сумма операции с округлением"
    ];

    private static readonly string[] AlfaBankHeaders =
    [
        "operationDate",
        "transactionDate",
        "accountName",
        "accountNumber",
        "cardName",
        "cardNumber",
        "merchant",
        "amount",
        "currency",
        "status",
        "category",
        "mcc",
        "type",
        "comment",
        "bonusValue",
        "bonusTitle"
    ];

    public static ImportPreset[] GetPresets() =>
    [
        new ImportPreset
        {
            Id = TBankPresetId,
            Name = "Т-Банк",
            MatchHeadersJson = ImportParseOptionsMapper.SerializeHeaders(TBankHeaders),
            ParseOptionsJson = ImportParseOptionsMapper.SerializeOptions(new CsvParseOptionsDto
            {
                Delimiter = ";",
                Culture = "ru-RU",
                HasHeaderRecord = true,
                ColumnMapping = new CsvColumnMappingDto
                {
                    Date = new CsvColumnFieldMappingDto { ColumnIndex = 0 },
                    Amount = new CsvColumnFieldMappingDto { ColumnIndex = 4 },
                    Currency = new CsvColumnFieldMappingDto { ColumnIndex = 5 },
                    CategoryName = new CsvColumnFieldMappingDto { ColumnIndex = 9 },
                    Description = new CsvColumnFieldMappingDto { ColumnIndex = 11 }
                }
            }),
            IsActive = true
        },
        new ImportPreset
        {
            Id = AlfaBankPresetId,
            Name = "Альфа-Банк",
            MatchHeadersJson = ImportParseOptionsMapper.SerializeHeaders(AlfaBankHeaders),
            ParseOptionsJson = ImportParseOptionsMapper.SerializeOptions(new CsvParseOptionsDto
            {
                Delimiter = ",",
                Culture = "ru-RU",
                NumberCulture = "Invariant",
                HasHeaderRecord = true,
                ColumnMapping = new CsvColumnMappingDto
                {
                    Date = new CsvColumnFieldMappingDto { ColumnIndex = 0 },
                    Amount = new CsvColumnFieldMappingDto { ColumnIndex = 7 },
                    Currency = new CsvColumnFieldMappingDto { ColumnIndex = 8 },
                    CategoryName = new CsvColumnFieldMappingDto { ColumnIndex = 10 },
                    Type = new CsvTypeFieldMappingDto
                    {
                        Column = new CsvColumnFieldMappingDto { ColumnIndex = 12 },
                        IncomeValues =
                        [
                            "income",
                            "приход",
                            "credit",
                            "зачисление",
                            "пополнение",
                            "доход"
                        ],
                        ExpenseValues =
                        [
                            "expense",
                            "расход",
                            "debit",
                            "списание"
                        ]
                    }
                }
            }),
            IsActive = true
        }
    ];

    public static async Task SeedAsync(AppDbContext dbContext)
    {
        var builtInPresets = GetPresets();
        var builtInIds = builtInPresets.Select(p => p.Id).ToHashSet();

        if (!await dbContext.ImportPresets.AnyAsync())
        {
            dbContext.ImportPresets.AddRange(builtInPresets);
            await dbContext.SaveChangesAsync();
            return;
        }

        var existing = await dbContext.ImportPresets
            .Where(p => builtInIds.Contains(p.Id))
            .ToListAsync();

        foreach (var preset in builtInPresets)
        {
            var entity = existing.FirstOrDefault(p => p.Id == preset.Id);
            if (entity is null)
            {
                dbContext.ImportPresets.Add(preset);
                continue;
            }

            entity.Name = preset.Name;
            entity.MatchHeadersJson = preset.MatchHeadersJson;
            entity.ParseOptionsJson = preset.ParseOptionsJson;
            entity.IsActive = true;
        }

        await dbContext.SaveChangesAsync();
    }
}
