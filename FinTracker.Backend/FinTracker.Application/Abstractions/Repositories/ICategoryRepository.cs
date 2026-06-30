using FinTracker.Domain.Models;

namespace FinTracker.Application.Abstractions.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
    Task<List<Category>> GetByNamesAsync(IEnumerable<string> names);
}
