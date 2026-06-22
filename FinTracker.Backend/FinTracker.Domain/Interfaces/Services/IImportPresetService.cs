using FinTracker.Domain.Dtos.Import;

namespace FinTracker.Domain.Interfaces.Services;

public interface IImportPresetService
{
    Task<IReadOnlyList<ImportPresetListItemDto>> GetAllAsync();
    Task<ImportPresetDto> GetByIdAsync(Guid id);
    Task<ImportPresetDto> CreateAsync(SaveImportPresetDto dto);
    Task<ImportPresetDto> UpdateAsync(Guid id, SaveImportPresetDto dto);
    Task DeleteAsync(Guid id);
    Task<CsvParseOptionsDto> GetParseOptionsAsync(Guid presetId);
    Task<ImportPresetMatchDto?> FindMatchAsync(string[] headers);
}
