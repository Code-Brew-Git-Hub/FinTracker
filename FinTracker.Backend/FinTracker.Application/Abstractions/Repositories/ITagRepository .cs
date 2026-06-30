using FinTracker.Domain.Models;

namespace FinTracker.Application.Abstractions.Repositories;

public interface ITagRepository : IRepository<Tag>
{
    Task<List<Tag>> GetByNamesAsync(List<string> names);
}
