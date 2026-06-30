using FinTracker.Domain.Models;

namespace FinTracker.Application.Abstractions.Repositories;

public interface IScopeRepository : IRepository<Scope>
{
    Task<Scope?> GetByNameAsync(string name);
}
