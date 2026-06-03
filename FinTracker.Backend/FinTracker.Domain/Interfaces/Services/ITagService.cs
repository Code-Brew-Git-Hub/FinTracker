using FinTracker.Domain.Models;

namespace FinTracker.Domain.Interfaces.Services;

public interface ITagService
{
    Task<List<Tag>> GetAllAsync();
    Task<Tag> CreateAsync(string name);
    Task DeleteAsync(Guid id);
}
