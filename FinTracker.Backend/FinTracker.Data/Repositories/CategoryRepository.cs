using FinTracker.Application.Abstractions.Repositories;
using FinTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.Data.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task AddAsync(Category entity)
    {
        await context.Categories
            .AddAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await GetByIdAsync(id);
        if (category != null)
            context.Categories
                .Remove(category);
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await context.Categories
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await context.Categories
            .FindAsync(id);
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await context.Categories
            .FirstOrDefaultAsync(category => category.Name == name);
    }

    public async Task<List<Category>> GetByNamesAsync(IEnumerable<string> names)
    {
        var nameList = names.Distinct().ToList();
        if (nameList.Count == 0)
            return [];

        return await context.Categories
            .Where(category => nameList.Contains(category.Name))
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category entity)
    {
        context.Categories.Update(entity);
    }
}
