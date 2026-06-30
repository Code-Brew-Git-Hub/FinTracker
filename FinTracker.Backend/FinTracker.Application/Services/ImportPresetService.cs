using FinTracker.Application.Abstractions.Repositories;
using FinTracker.Application.Abstractions.Services;
using FinTracker.Domain.Dtos.Import;
using FinTracker.Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FinTracker.Application.Services;

public class ImportPresetService(
    IImportPresetRepository repository,
    IMemoryCache memoryCache) : IImportPresetService
{
    private const string AllPresetsCacheKey = "import-presets:all";

    private static readonly MemoryCacheEntryOptions CacheEntryOptions = new MemoryCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromHours(1))
        .SetPriority(CacheItemPriority.Normal);

    public async Task<IReadOnlyList<ImportPresetListItemDto>> GetAllAsync()
    {
        if (memoryCache.TryGetValue(AllPresetsCacheKey, out IReadOnlyList<ImportPresetListItemDto>? cached)
            && cached is not null)
        {
            return cached;
        }

        var presets = await repository.GetAllActiveAsync();
        var result = presets
            .Select(p => new ImportPresetListItemDto { Id = p.Id, Name = p.Name })
            .ToList();

        memoryCache.Set(AllPresetsCacheKey, result, CacheEntryOptions);
        return result;
    }

    public async Task<ImportPresetDto> GetByIdAsync(Guid id)
    {
        var preset = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Пресет импорта '{id}' не найден");

        return ToDto(preset);
    }

    public async Task<ImportPresetDto> CreateAsync(SaveImportPresetDto dto)
    {
        ValidateSaveDto(dto);

        var name = dto.Name.Trim();
        if (await repository.ExistsByNameAsync(name))
            throw new ArgumentException($"Пресет с именем '{name}' уже существует");

        var preset = new ImportPreset
        {
            Id = Guid.NewGuid(),
            Name = name,
            MatchHeadersJson = ImportParseOptionsMapper.SerializeHeaders(dto.MatchHeaders),
            ParseOptionsJson = ImportParseOptionsMapper.SerializeOptions(dto.ParseOptions),
            IsActive = true
        };

        await repository.AddAsync(preset);
        await repository.SaveChangesAsync();
        InvalidateCache();

        return ToDto(preset);
    }

    public async Task<ImportPresetDto> UpdateAsync(Guid id, SaveImportPresetDto dto)
    {
        ValidateSaveDto(dto);

        var preset = await repository.GetByIdTrackedAsync(id)
            ?? throw new KeyNotFoundException($"Пресет импорта '{id}' не найден");

        var name = dto.Name.Trim();
        if (await repository.ExistsByNameAsync(name, id))
            throw new ArgumentException($"Пресет с именем '{name}' уже существует");

        preset.Name = name;
        preset.MatchHeadersJson = ImportParseOptionsMapper.SerializeHeaders(dto.MatchHeaders);
        preset.ParseOptionsJson = ImportParseOptionsMapper.SerializeOptions(dto.ParseOptions);

        await repository.SaveChangesAsync();
        InvalidateCache(id);

        return ToDto(preset);
    }

    public async Task DeleteAsync(Guid id)
    {
        var preset = await repository.GetByIdTrackedAsync(id)
            ?? throw new KeyNotFoundException($"Пресет импорта '{id}' не найден");

        preset.IsActive = false;
        await repository.SaveChangesAsync();
        InvalidateCache(id);
    }

    public async Task<CsvParseOptionsDto> GetParseOptionsAsync(Guid presetId)
    {
        var cacheKey = GetPresetOptionsCacheKey(presetId);
        if (memoryCache.TryGetValue(cacheKey, out CsvParseOptionsDto? cached) && cached is not null)
            return cached;

        var preset = await repository.GetByIdAsync(presetId)
            ?? throw new KeyNotFoundException($"Пресет импорта '{presetId}' не найден");

        var options = ImportParseOptionsMapper.DeserializeOptions(preset.ParseOptionsJson);
        memoryCache.Set(cacheKey, options, CacheEntryOptions);
        return options;
    }

    public async Task<ImportPresetMatchDto?> FindMatchAsync(string[] headers)
    {
        var presets = await repository.GetAllActiveAsync();

        foreach (var preset in presets)
        {
            var signature = ImportParseOptionsMapper.DeserializeHeaders(preset.MatchHeadersJson);
            if (!HeadersMatch(headers, signature))
                continue;

            return new ImportPresetMatchDto
            {
                Id = preset.Id,
                Name = preset.Name,
                ParseOptions = ImportParseOptionsMapper.DeserializeOptions(preset.ParseOptionsJson)
            };
        }

        return null;
    }

    private static ImportPresetDto ToDto(ImportPreset preset) => new()
    {
        Id = preset.Id,
        Name = preset.Name,
        MatchHeaders = ImportParseOptionsMapper.DeserializeHeaders(preset.MatchHeadersJson),
        ParseOptions = ImportParseOptionsMapper.DeserializeOptions(preset.ParseOptionsJson)
    };

    private static void ValidateSaveDto(SaveImportPresetDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Имя пресета обязательно");

        if (dto.Name.Trim().Length > 100)
            throw new ArgumentException("Имя пресета не должно превышать 100 символов");

        if (dto.MatchHeaders is null || dto.MatchHeaders.Length == 0)
            throw new ArgumentException("Укажите заголовки для сопоставления");

        if (dto.ParseOptions?.ColumnMapping is null)
            throw new ArgumentException("В parseOptions должно быть указано поле columnMapping");

        ValidateRequiredField(dto.ParseOptions.ColumnMapping.Date, "date");
        ValidateRequiredField(dto.ParseOptions.ColumnMapping.Amount, "amount");
        ValidateRequiredField(dto.ParseOptions.ColumnMapping.Currency, "currency");
        ValidateRequiredField(dto.ParseOptions.ColumnMapping.CategoryName, "categoryName");

        if (dto.ParseOptions.ColumnMapping.Description != null)
            ValidateOptionalField(dto.ParseOptions.ColumnMapping.Description, "description");

        if (dto.ParseOptions.ColumnMapping.Type != null)
            ValidateRequiredField(dto.ParseOptions.ColumnMapping.Type.Column, "type");
    }

    private static void ValidateRequiredField(CsvColumnFieldMappingDto field, string fieldName)
    {
        if (!IsFieldDefined(field))
            throw new ArgumentException($"Маппинг для поля '{fieldName}' не задан");
    }

    private static void ValidateOptionalField(CsvColumnFieldMappingDto field, string fieldName)
    {
        if (field.ColumnIndex == null && string.IsNullOrWhiteSpace(field.ColumnName))
            return;

        if (!IsFieldDefined(field))
            throw new ArgumentException($"Маппинг для поля '{fieldName}' не задан");
    }

    private static bool IsFieldDefined(CsvColumnFieldMappingDto field) =>
        field.ColumnIndex.HasValue || !string.IsNullOrWhiteSpace(field.ColumnName);

    private static bool HeadersMatch(string[] fileHeaders, string[] signatureHeaders)
    {
        if (fileHeaders.Length < signatureHeaders.Length)
            return false;

        for (var i = 0; i < signatureHeaders.Length; i++)
        {
            if (!string.Equals(fileHeaders[i], signatureHeaders[i], StringComparison.Ordinal))
                return false;
        }

        return true;
    }

    private void InvalidateCache(Guid? presetId = null)
    {
        memoryCache.Remove(AllPresetsCacheKey);

        if (presetId.HasValue)
            memoryCache.Remove(GetPresetOptionsCacheKey(presetId.Value));
    }

    private static string GetPresetOptionsCacheKey(Guid presetId) => $"import-presets:{presetId}:options";
}
