using FinTracker.Application.Abstractions.Repositories;
using FinTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.Data.Repositories;

public class ImportPresetRepository(AppDbContext dbContext) : IImportPresetRepository
{
    public async Task<IReadOnlyList<ImportPreset>> GetAllActiveAsync() =>
        await dbContext.ImportPresets
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

    public async Task<ImportPreset?> GetByIdAsync(Guid id) =>
        await dbContext.ImportPresets
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

    public async Task<ImportPreset?> GetByIdTrackedAsync(Guid id) =>
        await dbContext.ImportPresets
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null) =>
        await dbContext.ImportPresets
            .AnyAsync(p => p.IsActive
                           && p.Name == name
                           && (!excludeId.HasValue || p.Id != excludeId.Value));

    public async Task AddAsync(ImportPreset preset) =>
        await dbContext.ImportPresets.AddAsync(preset);

    public Task SaveChangesAsync() =>
        dbContext.SaveChangesAsync();
}
