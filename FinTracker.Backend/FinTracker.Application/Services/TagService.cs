using FinTracker.Domain.Dtos.Tags;
using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Interfaces.Services;
using FinTracker.Domain.Models;
using MapsterMapper;

namespace FinTracker.Application.Services;

public class TagService(ITagRepository tagRepository,
    IMapper mapper) : ITagService
{
    public async Task<TagDto> CreateAsync(string name)
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

        return mapper.Map<TagDto>(tag);
    }

    public async Task DeleteAsync(Guid id)
    {
        await tagRepository.DeleteAsync(id);
        await tagRepository.SaveChangesAsync();
    }

    public async Task<List<TagDto>> GetAllAsync()
    {
        var tags = await tagRepository.GetAllAsync();

        return mapper.Map<List<TagDto>>(tags);
    }
}
