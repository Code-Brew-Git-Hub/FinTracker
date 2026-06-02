using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.Data.Repositories;

public class ScopeRepository(AppDbContext context) : IScopeRepository
{
    public async Task AddAsync(Scope entity)
    {
        context.Scopes.Add(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var scope = await GetByIdAsync(id);
        if (scope != null)
            context.Scopes
                .Remove(scope);
    }

    public async Task<List<Scope>> GetAllAsync()
    {
        return await context.Scopes
            .ToListAsync();
    }

    public async Task<Scope?> GetByIdAsync(Guid id)
    {
        return await context.Scopes.FindAsync(id);
    }

    public async Task<Scope?> GetByNameAsync(string name)
    {
        return await context.Scopes
            .FirstOrDefaultAsync(sc => sc.Name == name);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Scope entity)
    {
        context.Scopes.Update(entity);
    }
}
