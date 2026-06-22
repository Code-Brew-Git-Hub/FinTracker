using System.Globalization;
using System.Linq.Expressions;
using CsvHelper.Configuration;
using FinTracker.Parser.Models;

namespace FinTracker.Parser;

public sealed class DynamicCsvMap : ClassMap<ParsedTransaction>
{
    public DynamicCsvMap(CsvColumnMapping mapping, CsvParseOptions options)
    {
        MapRequired(m => m.Date, mapping.Date, map =>
        {
            if (!string.IsNullOrWhiteSpace(options.DateFormat))
                map.TypeConverterOption.Format(options.DateFormat);
        });

        MapRequired(m => m.Amount, mapping.Amount, map =>
        {
            var numberCulture = ResolveCulture(options.NumberCulture ?? options.Culture);
            map.TypeConverterOption.NumberStyles(NumberStyles.Any);
            map.TypeConverterOption.CultureInfo(numberCulture);
        });

        MapRequired(m => m.Currency, mapping.Currency);
        MapRequired(m => m.CategoryName, mapping.CategoryName);

        if (mapping.Description != null
            && (mapping.Description.ColumnIndex.HasValue || !string.IsNullOrWhiteSpace(mapping.Description.ColumnName)))
        {
            MapOptional(m => m.Description, mapping.Description);
        }

        if (mapping.Type != null
            && (mapping.Type.Column.ColumnIndex.HasValue || !string.IsNullOrWhiteSpace(mapping.Type.Column.ColumnName)))
        {
            MapOptional(m => m.TypeRaw, mapping.Type.Column);
        }
    }

    private void MapRequired<TMember>(
        Expression<Func<ParsedTransaction, TMember>> member,
        CsvColumnFieldMapping fieldMapping,
        Action<MemberMap<ParsedTransaction, TMember>>? configure = null)
    {
        var map = Map(member);
        ApplyColumnTarget(map, fieldMapping);
        configure?.Invoke(map);
    }

    private void MapOptional<TMember>(
        Expression<Func<ParsedTransaction, TMember>> member,
        CsvColumnFieldMapping fieldMapping)
    {
        var map = Map(member);
        ApplyColumnTarget(map, fieldMapping);
    }

    private static void ApplyColumnTarget<TMember>(
        MemberMap<ParsedTransaction, TMember> map,
        CsvColumnFieldMapping field)
    {
        if (field.ColumnIndex.HasValue)
            map.Index(field.ColumnIndex.Value);
        else if (!string.IsNullOrWhiteSpace(field.ColumnName))
            map.Name(field.ColumnName);
    }

    private static CultureInfo ResolveCulture(string cultureName) =>
        cultureName.Equals("Invariant", StringComparison.OrdinalIgnoreCase)
            ? CultureInfo.InvariantCulture
            : CultureInfo.GetCultureInfo(cultureName);
}
