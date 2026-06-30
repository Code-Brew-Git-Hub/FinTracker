using FinTracker.Application.Services;
using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Models;
using MapsterMapper;
using Moq;

namespace Application.UnitTests.Services;

public class ScopeServiceTests
{
    private readonly Mock<IScopeRepository> _scopeRepository = new();
    private readonly Mock<ITransactionRepository> _transactionRepository = new();
    private readonly IMapper _mapper = TestMapperFactory.Create();
    private readonly ScopeService _sut;

    public ScopeServiceTests()
    {
        _sut = new ScopeService(_scopeRepository.Object, _transactionRepository.Object, _mapper);
    }

    [Fact]
    public async Task CreateAsync_WhenNameIsUnique_CreatesScope()
    {
        _scopeRepository.Setup(r => r.GetByNameAsync("Trip")).ReturnsAsync((Scope?)null);
        _scopeRepository.Setup(r => r.AddAsync(It.IsAny<Scope>())).Returns(Task.CompletedTask);
        _scopeRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync("Trip");

        Assert.Equal("Trip", result.Name);
        _scopeRepository.Verify(r => r.AddAsync(It.Is<Scope>(s => s.Name == "Trip")), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenScopeExists_ThrowsArgumentException()
    {
        _scopeRepository.Setup(r => r.GetByNameAsync("Trip"))
            .ReturnsAsync(new Scope { Id = Guid.NewGuid(), Name = "Trip" });

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync("Trip"));

        Assert.Contains("already exists", ex.Message);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsDto()
    {
        var id = Guid.NewGuid();
        _scopeRepository.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Scope { Id = id, Name = "Trip" });

        var result = await _sut.GetByIdAsync(id);

        Assert.Equal(id, result.Id);
        Assert.Equal("Trip", result.Name);
    }

    [Fact]
    public async Task GetTransactionsAsync_ReturnsTransactionsForScope()
    {
        var scopeId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        _scopeRepository.Setup(r => r.GetByIdAsync(scopeId))
            .ReturnsAsync(new Scope { Id = scopeId, Name = "Trip" });
        _transactionRepository.Setup(r => r.GetByScopeIdAsync(scopeId))
            .ReturnsAsync(
            [
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = -100,
                    Currency = "RUB",
                    CategoryId = categoryId,
                    ScopeId = scopeId,
                    TransactionTags = []
                }
            ]);

        var result = await _sut.GetTransactionsAsync(scopeId);

        Assert.Single(result);
        Assert.Equal("Expense", result[0].Type);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesName()
    {
        var id = Guid.NewGuid();
        var scope = new Scope { Id = id, Name = "Old" };
        _scopeRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(scope);
        _scopeRepository.Setup(r => r.UpdateAsync(scope)).Returns(Task.CompletedTask);
        _scopeRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _sut.UpdateAsync(id, "New");

        Assert.Equal("New", result.Name);
        Assert.Equal("New", scope.Name);
    }
}
