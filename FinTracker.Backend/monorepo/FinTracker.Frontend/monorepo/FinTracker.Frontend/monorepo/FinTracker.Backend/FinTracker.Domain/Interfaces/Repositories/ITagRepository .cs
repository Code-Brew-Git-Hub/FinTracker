using FinTracker.Domain.Models;

namespace FinTracker.Domain.Interfaces.Repositories;

public interface ITagRepository : IRepository<Tag>
{
    Task<List<Tag>> GetByNamesAsync(List<string> names);
}
