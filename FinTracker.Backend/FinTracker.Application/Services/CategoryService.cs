using FinTracker.Domain.Dtos.Categories;
using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Interfaces.Services;
using FinTracker.Domain.Models;
using MapsterMapper;
using Microsoft.Extensions.Caching.Memory;

namespace FinTracker.Application.Services;

public class CategoryService(
    ICategoryRepository categoryRepository,
    IMemoryCache memoryCache,
    IMapper mapper) : ICategoryService
{
    public async Task<CategoryDto> CreateAsync(string name)
    {
        var newCategory = new Category()
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        await categoryRepository.AddAsync(newCategory);
        await categoryRepository.SaveChangesAsync();

        return mapper.Map<CategoryDto>(newCategory);
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category != null)
            memoryCache.Remove(category.Name);

        await categoryRepository.DeleteAsync(id);
        await categoryRepository.SaveChangesAsync();
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await categoryRepository.GetAllAsync();
        return mapper.Map<List<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> GetByIdAsync(Guid id)
    {
        var category = await categoryRepository.EnsureExistsAsync(id);
        return mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, string name)
    {
        var oldCategory = await categoryRepository.EnsureExistsAsync(id);

        var oldName = oldCategory.Name;

        oldCategory.Name = name;
        await categoryRepository.UpdateAsync(oldCategory);
        await categoryRepository.SaveChangesAsync();

        memoryCache.Remove(oldName);
        memoryCache.Remove(name);

        return mapper.Map<CategoryDto>(oldCategory);
    }
}
