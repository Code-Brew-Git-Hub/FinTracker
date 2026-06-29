using FinTracker.Domain.Dtos.Categories;

namespace FinTracker.Domain.Interfaces.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto> GetByIdAsync(Guid id);
    Task<CategoryDto> CreateAsync(string name);
    Task<CategoryDto> UpdateAsync(Guid id, string name);
    Task DeleteAsync(Guid id);
}
