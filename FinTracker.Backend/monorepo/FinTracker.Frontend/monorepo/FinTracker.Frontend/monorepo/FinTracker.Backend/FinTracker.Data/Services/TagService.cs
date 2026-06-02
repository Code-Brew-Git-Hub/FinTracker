
using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Interfaces.Services;
using FinTracker.Domain.Models;

namespace FinTracker.Data.Services;

public class TagService(ITagRepository tagRepository) : ITagService
{
    public async Task<Tag> CreateAsync(string name)
    {
        var existing = await tagRepository.GetByNamesAsync([name]);
        if (existing.Any())
            throw new ArgumentException($"Tag '{name}' already exists");

        var tag = new Tag()
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        await tagRepository.AddAsync(tag);
        await tagRepository.SaveChangesAsync();

        return tag;
    }

    public async Task DeleteAsync(Guid id)
    {
        await tagRepository.DeleteAsync(id);
        await tagRepository.SaveChangesAsync();
    }

    public async Task<List<Tag>> GetAllAsync()
    {
        return await tagRepository.GetAllAsync();
    }
}
