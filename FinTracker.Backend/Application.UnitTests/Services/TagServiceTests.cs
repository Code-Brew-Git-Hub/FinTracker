using FinTracker.Application.Abstractions.Repositories;
using FinTracker.Application.Services;
using FinTracker.Domain.Models;
using MapsterMapper;
using Moq;

namespace Application.UnitTests.Services;

public class TagServiceTests
{
    private readonly Mock<ITagRepository> _repository = new();
    private readonly IMapper _mapper = TestMapperFactory.Create();
    private readonly TagService _sut;

    public TagServiceTests()
    {
        _sut = new TagService(_repository.Object, _mapper);
    }

    [Fact]
    public async Task CreateAsync_WhenNameIsUnique_CreatesTag()
    {
        _repository.Setup(r => r.GetByNamesAsync(It.IsAny<List<string>>()))
            .ReturnsAsync([]);
        _repository.Setup(r => r.AddAsync(It.IsAny<Tag>())).Returns(Task.CompletedTask);
        _repository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync("vacation");

        Assert.Equal("vacation", result.Name);
        _repository.Verify(r => r.AddAsync(It.Is<Tag>(t => t.Name == "vacation")), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenTagExists_ThrowsArgumentException()
    {
        _repository.Setup(r => r.GetByNamesAsync(It.IsAny<List<string>>()))
            .ReturnsAsync([new Tag { Id = Guid.NewGuid(), Name = "vacation" }]);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync("vacation"));

        Assert.Contains("already exists", ex.Message);
        _repository.Verify(r => r.AddAsync(It.IsAny<Tag>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedTags()
    {
        var tags = new List<Tag>
        {
            new() { Id = Guid.NewGuid(), Name = "work" },
            new() { Id = Guid.NewGuid(), Name = "home" }
        };
        _repository.Setup(r => r.GetAllAsync()).ReturnsAsync(tags);

        var result = await _sut.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("work", result[0].Name);
    }

    [Fact]
    public async Task DeleteAsync_DeletesTag()
    {
        var id = Guid.NewGuid();
        _repository.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
        _repository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _sut.DeleteAsync(id);

        _repository.Verify(r => r.DeleteAsync(id), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
