using FinTracker.Domain.Dtos.Scopes;
using FinTracker.Domain.Dtos.Transactions;
using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Interfaces.Services;
using FinTracker.Domain.Models;
using MapsterMapper;

namespace FinTracker.Application.Services;

public class ScopeService(IScopeRepository scopeRepository,
    ITransactionRepository transactionRepository,
    IMapper mapper) : IScopeService
{
    public async Task<ScopeDto> CreateAsync(string name)
    {
        var existing = await scopeRepository.GetByNameAsync(name);
        if (existing != null)
            throw new ArgumentException($"Scope '{name}' already exists");

        var newScope = new Scope()
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        await scopeRepository.AddAsync(newScope);
        await scopeRepository.SaveChangesAsync();

        return mapper.Map<ScopeDto>(newScope);
    }

    public async Task DeleteAsync(Guid id)
    {
        await scopeRepository.DeleteAsync(id);
        await scopeRepository.SaveChangesAsync();
    }

    public async Task<List<ScopeDto>> GetAllAsync()
    {
        var scopes = await scopeRepository.GetAllAsync();
        return mapper.Map<List<ScopeDto>>(scopes);
    }

    public async Task<ScopeDto> GetByIdAsync(Guid id)
    {
        var scope = await scopeRepository.EnsureExistsAsync(id);
        return mapper.Map<ScopeDto>(scope);
    }

    public async Task<List<TransactionDto>> GetTransactionsAsync(Guid scopeId)
    {
        var scope = await scopeRepository.EnsureExistsAsync(scopeId);

        var transactions = await transactionRepository.GetByScopeIdAsync(scopeId);

        return mapper.Map<List<TransactionDto>>(transactions);
    }

    public async Task<ScopeDto> UpdateAsync(Guid id, string name)
    {
        var scope = await scopeRepository.EnsureExistsAsync(id);

        scope.Name = name;

        await scopeRepository.UpdateAsync(scope);
        await scopeRepository.SaveChangesAsync();

        return mapper.Map<ScopeDto>(scope);
    }
}
