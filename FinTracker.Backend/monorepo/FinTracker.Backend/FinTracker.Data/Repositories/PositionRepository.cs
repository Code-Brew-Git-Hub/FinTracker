using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.Data.Repositories;

public class PositionRepository(AppDbContext context) : IPositionRepository
{
    public async Task AddAsync(Position entity)
    {
        await context.TransactionItems.AddAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var item = await GetByIdAsync(id);
        if (item != null)
            context.TransactionItems.Remove(item);
    }

    public async Task<List<Position>> GetAllAsync()
    {
        return await context.TransactionItems.ToListAsync();
    }

    public async Task<List<Position>> GetAllByTransactionIdAsync(Guid transactionId)
    {
        return await context.TransactionItems
            .Include(ti => ti.Category)
            .Where(ti => ti.TransactionId == transactionId)
            .ToListAsync();
    }

    public async Task<Position?> GetByIdAsync(Guid id)
    {
        return await context.TransactionItems.FindAsync(id);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Position entity)
    {
        context.TransactionItems.Update(entity);
    }
}
