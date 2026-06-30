using System.Text.Json;
using FinTracker.Domain.Dtos.Import;
using FinTracker.Parser.Models;

namespace FinTracker.Application;

public static class ImportParseOptionsMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public static CsvParseOptions ToParserOptions(CsvParseOptionsDto dto) => new()
    {
        Delimiter = dto.Delimiter,
        Culture = dto.Culture,
        DateFormat = dto.DateFormat,
        NumberCulture = dto.NumberCulture,
        HasHeaderRecord = dto.HasHeaderRecord,
        ColumnMapping = ToColumnMapping(dto.ColumnMapping)
    };

    public static CsvColumnMapping ToColumnMapping(CsvColumnMappingDto dto) => new()
    {
        Date = ToFieldMapping(dto.Date),
        Amount = ToFieldMapping(dto.Amount),
        Currency = ToFieldMapping(dto.Currency),
        CategoryName = ToFieldMapping(dto.CategoryName),
        Description = dto.Description != null ? ToFieldMapping(dto.Description) : null,
        Type = dto.Type != null ? ToTypeMapping(dto.Type) : null
    };

    public static CsvColumnMappingDto ToColumnMappingDto(CsvColumnMapping mapping) => new()
    {
        Date = ToFieldMappingDto(mapping.Date),
        Amount = ToFieldMappingDto(mapping.Amount),
        Currency = ToFieldMappingDto(mapping.Currency),
        CategoryName = ToFieldMappingDto(mapping.CategoryName),
        Description = mapping.Description != null ? ToFieldMappingDto(mapping.Description) : null,
        Type = mapping.Type != null ? ToTypeMappingDto(mapping.Type) : null
    };

    public static string SerializeOptions(CsvParseOptionsDto dto) =>
        JsonSerializer.Serialize(dto, JsonOptions);

    public static string SerializeHeaders(string[] headers) =>
        JsonSerializer.Serialize(headers, JsonOptions);

    public static CsvParseOptionsDto DeserializeOptions(string json) =>
        JsonSerializer.Deserialize<CsvParseOptionsDto>(json, JsonOptions)
        ?? throw new InvalidOperationException("Не удалось десериализовать настройки пресета импорта");

    public static string[] DeserializeHeaders(string json) =>
        JsonSerializer.Deserialize<string[]>(json, JsonOptions)
        ?? throw new InvalidOperationException("Не удалось десериализовать сигнатуру заголовков пресета");

    private static CsvTypeFieldMapping ToTypeMapping(CsvTypeFieldMappingDto dto) => new()
    {
        Column = ToFieldMapping(dto.Column),
        IncomeValues = dto.IncomeValues?.Count > 0 ? dto.IncomeValues : new CsvTypeFieldMapping().IncomeValues,
        ExpenseValues = dto.ExpenseValues?.Count > 0 ? dto.ExpenseValues : new CsvTypeFieldMapping().ExpenseValues
    };

    private static CsvTypeFieldMappingDto ToTypeMappingDto(CsvTypeFieldMapping mapping) => new()
    {
        Column = ToFieldMappingDto(mapping.Column),
        IncomeValues = mapping.IncomeValues,
        ExpenseValues = mapping.ExpenseValues
    };

    private static CsvColumnFieldMapping ToFieldMapping(CsvColumnFieldMappingDto dto) => new()
    {
        ColumnName = dto.ColumnName,
        ColumnIndex = dto.ColumnIndex
    };

    private static CsvColumnFieldMappingDto ToFieldMappingDto(CsvColumnFieldMapping mapping) => new()
    {
        ColumnName = mapping.ColumnName,
        ColumnIndex = mapping.ColumnIndex
    };
}
