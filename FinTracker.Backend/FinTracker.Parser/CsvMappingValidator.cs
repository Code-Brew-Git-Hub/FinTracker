using FinTracker.Parser.Models;

namespace FinTracker.Parser;

public static class CsvMappingValidator
{
    public static void Validate(string[] headers, CsvColumnMapping mapping)
    {
        ValidateRequiredField(headers, mapping.Date, nameof(mapping.Date));
        ValidateRequiredField(headers, mapping.Amount, nameof(mapping.Amount));
        ValidateRequiredField(headers, mapping.Currency, nameof(mapping.Currency));
        ValidateRequiredField(headers, mapping.CategoryName, nameof(mapping.CategoryName));

        if (mapping.Description != null)
            ValidateOptionalField(headers, mapping.Description, nameof(mapping.Description));

        if (mapping.Type != null)
            ValidateOptionalField(headers, mapping.Type.Column, nameof(mapping.Type));
    }

    private static void ValidateRequiredField(string[] headers, CsvColumnFieldMapping field, string fieldName)
    {
        if (!IsFieldValid(headers, field))
            throw new ArgumentException($"Маппинг для поля '{fieldName}' не задан или колонка не найдена в заголовках");
    }

    private static void ValidateOptionalField(string[] headers, CsvColumnFieldMapping field, string fieldName)
    {
        if (field.ColumnIndex == null && string.IsNullOrWhiteSpace(field.ColumnName))
            return;

        if (!IsFieldValid(headers, field))
            throw new ArgumentException($"Колонка для поля '{fieldName}' не найдена в заголовках");
    }

    private static bool IsFieldValid(string[] headers, CsvColumnFieldMapping field)
    {
        if (field.ColumnIndex.HasValue)
            return field.ColumnIndex.Value >= 0 && field.ColumnIndex.Value < headers.Length;

        if (!string.IsNullOrWhiteSpace(field.ColumnName))
            return headers.Contains(field.ColumnName);

        return false;
    }
}
