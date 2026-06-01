using FinTracker.Domain.Models;

namespace FinTracker.Domain.Interfaces.Repositories;

public interface IScopeRepository : IRepository<Scope>
{
    Task<Scope?> GetByNameAsync(string name);
}
