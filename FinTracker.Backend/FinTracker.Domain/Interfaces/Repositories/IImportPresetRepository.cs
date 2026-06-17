using FinTracker.Domain.Models;

namespace FinTracker.Domain.Interfaces.Repositories;

public interface IImportPresetRepository
{
    Task<IReadOnlyList<ImportPreset>> GetAllActiveAsync();
    Task<ImportPreset?> GetByIdAsync(Guid id);
    Task<ImportPreset?> GetByIdTrackedAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
    Task AddAsync(ImportPreset preset);
    Task SaveChangesAsync();
}
