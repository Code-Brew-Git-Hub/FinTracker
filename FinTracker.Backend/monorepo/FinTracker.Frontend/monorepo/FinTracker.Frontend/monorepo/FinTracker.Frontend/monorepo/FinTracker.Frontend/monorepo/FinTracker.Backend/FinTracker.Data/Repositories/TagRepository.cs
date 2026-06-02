
using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.Data.Repositories;

public class TagRepository(AppDbContext context) : ITagRepository
{
    public async Task AddAsync(Tag entity)
    {
        await context.Tags
            .AddAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var tag = await GetByIdAsync(id);
        if (tag != null)
            context.Tags
                .Remove(tag);
    }

    public async Task<List<Tag>> GetAllAsync()
    {
        return await context.Tags
            .ToListAsync();
    }

    public async Task<Tag?> GetByIdAsync(Guid id)
    {
        return await context.Tags
            .FindAsync(id);
    }

    public async Task<List<Tag>> GetByNamesAsync(List<string> names)
    {
        return await context.Tags
            .Where(tg => names.Contains(tg.Name))
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Tag entity)
    {
        context.Tags.Update(entity);
    }
}
