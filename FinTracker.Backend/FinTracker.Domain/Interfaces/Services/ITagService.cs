using FinTracker.Domain.Dtos.Tags;
using FinTracker.Domain.Models;

namespace FinTracker.Domain.Interfaces.Services;

public interface ITagService
{
    Task<List<TagDto>> GetAllAsync();
    Task<TagDto> CreateAsync(string name);
    Task DeleteAsync(Guid id);
}
