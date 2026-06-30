using FinTracker.Application.Abstractions.Repositories;
using FinTracker.Application.Services;
using FinTracker.Domain.Models;
using MapsterMapper;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Application.UnitTests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _repository = new();
    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
    private readonly IMapper _mapper = TestMapperFactory.Create();
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _sut = new CategoryService(_repository.Object, _memoryCache, _mapper);
    }

    [Fact]
    public async Task CreateAsync_AddsCategoryAndReturnsDto()
    {
        Category? captured = null;
        _repository
            .Setup(r => r.AddAsync(It.IsAny<Category>()))
            .Callback<Category>(c => captured = c)
            .Returns(Task.CompletedTask);
        _repository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync("Food");

        Assert.NotNull(captured);
        Assert.Equal("Food", captured!.Name);
        Assert.Equal("Food", result.Name);
        _repository.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedCategories()
    {
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "Food" },
            new() { Id = Guid.NewGuid(), Name = "Transport" }
        };
        _repository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        var result = await _sut.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Name == "Food");
        Assert.Contains(result, c => c.Name == "Transport");
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsDto()
    {
        var id = Guid.NewGuid();
        _repository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Category { Id = id, Name = "Food" });

        var result = await _sut.GetByIdAsync(id);

        Assert.Equal(id, result.Id);
        Assert.Equal("Food", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Category?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetByIdAsync(id));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesNameAndClearsCache()
    {
        var id = Guid.NewGuid();
        var category = new Category { Id = id, Name = "Old" };
        _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);
        _repository.Setup(r => r.UpdateAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
        _repository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        _memoryCache.Set("Old", "cached");
        _memoryCache.Set("New", "cached");

        var result = await _sut.UpdateAsync(id, "New");

        Assert.Equal("New", result.Name);
        Assert.False(_memoryCache.TryGetValue("Old", out _));
        Assert.False(_memoryCache.TryGetValue("New", out _));
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ClearsCacheAndDeletes()
    {
        var id = Guid.NewGuid();
        _repository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Category { Id = id, Name = "Food" });
        _repository.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
        _repository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        _memoryCache.Set("Food", "cached");

        await _sut.DeleteAsync(id);

        Assert.False(_memoryCache.TryGetValue("Food", out _));
        _repository.Verify(r => r.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_StillDeletesWithoutCacheClear()
    {
        var id = Guid.NewGuid();
        _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Category?)null);
        _repository.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
        _repository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        _memoryCache.Set("Food", "cached");

        await _sut.DeleteAsync(id);

        Assert.True(_memoryCache.TryGetValue("Food", out _));
    }
}
