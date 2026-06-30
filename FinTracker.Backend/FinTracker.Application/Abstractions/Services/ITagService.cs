using FinTracker.Domain.Dtos.Tags;

namespace FinTracker.Application.Abstractions.Services;

public interface ITagService
{
    Task<List<TagDto>> GetAllAsync();
    Task<TagDto> CreateAsync(string name);
    Task DeleteAsync(Guid id);
}
