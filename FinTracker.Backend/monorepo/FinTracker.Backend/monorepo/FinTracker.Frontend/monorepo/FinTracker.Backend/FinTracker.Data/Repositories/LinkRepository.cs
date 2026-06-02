using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.Data.Repositories;

public class LinkRepository(AppDbContext context) : ILinkRepository
{
    public async Task AddAsync(TransactionLink entity)
    {
        await context.AddAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var link = await GetByIdAsync(id);
        if (link != null)
            context.TransactionLinks.Remove(link);
    }

    public async Task<List<TransactionLink>> GetAllAsync()
    {
        return await context.TransactionLinks.ToListAsync();
    }

    public async Task<TransactionLink?> GetByIdAsync(Guid id)
    {
        return await context.TransactionLinks.FindAsync(id);
    }

    public async Task<TransactionLink?> GetByIdWithEntriesAsync(Guid id)
    {
        return await context.TransactionLinks
            .Include(tl => tl.Entries)
                .ThenInclude(e => e.Transaction)
            .FirstOrDefaultAsync(tl => tl.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TransactionLink entity)
    {
        context.TransactionLinks.Update(entity);
    }
}
