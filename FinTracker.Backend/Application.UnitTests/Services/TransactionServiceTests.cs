using FinTracker.Application.Abstractions.Repositories;
using FinTracker.Application.Services;
using FinTracker.Domain.Dtos.Transactions;
using FinTracker.Domain.Enums;
using FinTracker.Domain.Models;
using FinTracker.Domain.Models.ModelsToHelp;
using MapsterMapper;
using Moq;

namespace Application.UnitTests.Services;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _repository = new();
    private readonly IMapper _mapper = TestMapperFactory.Create();
    private readonly TransactionService _sut;

    public TransactionServiceTests()
    {
        _sut = new TransactionService(_repository.Object, _mapper);
    }

    [Fact]
    public async Task CreateAsync_NegativeAmount_SetsExpenseType()
    {
        var categoryId = Guid.NewGuid();
        _repository.Setup(r => r.AddAsync(It.IsAny<Transaction>())).Returns(Task.CompletedTask);
        _repository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(new CreateTransactionDto
        {
            Amount = -500,
            Currency = "RUB",
            DateUtc = DateTime.UtcNow,
            CategoryId = categoryId
        });

        Assert.Equal("Expense", result.Type);
        _repository.Verify(r => r.AddAsync(It.Is<Transaction>(t =>
            t.Type == TransactionType.Expense && t.Amount == -500)), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_PositiveAmount_SetsIncomeType()
    {
        var categoryId = Guid.NewGuid();
        _repository.Setup(r => r.AddAsync(It.IsAny<Transaction>())).Returns(Task.CompletedTask);
        _repository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(new CreateTransactionDto
        {
            Amount = 1000,
            Currency = "RUB",
            DateUtc = DateTime.UtcNow,
            CategoryId = categoryId
        });

        Assert.Equal("Income", result.Type);
    }

    [Fact]
    public async Task UpdateAsync_DeleteScopeWithoutScope_Throws()
    {
        var id = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = id,
            Amount = -100,
            CategoryId = Guid.NewGuid(),
            ScopeId = null,
            TransactionTags = []
        };
        _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(transaction);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.UpdateAsync(id, new UpdateTransactionDto { DeleteScope = true }));

        Assert.Contains("doesn't have scope", ex.Message);
    }

    [Fact]
    public async Task SoftDeleteAsync_SetsIsDeleted()
    {
        var id = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = id,
            Amount = -100,
            CategoryId = Guid.NewGuid(),
            IsDeleted = false,
            TransactionTags = []
        };
        _repository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(transaction);
        _repository.Setup(r => r.UpdateAsync(transaction)).Returns(Task.CompletedTask);
        _repository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _sut.SoftDeleteAsync(id);

        Assert.True(transaction.IsDeleted);
    }

    [Fact]
    public async Task GetFilteredAsync_ReturnsPagedResponse()
    {
        var categoryId = Guid.NewGuid();
        var filter = new TransactionFilter { Page = 1, PageSize = 10 };
        var transactions = new List<Transaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Amount = -100,
                Currency = "RUB",
                CategoryId = categoryId,
                TransactionTags = []
            }
        };

        _repository.Setup(r => r.GetFilteredCountAsync(filter, false)).ReturnsAsync(1);
        _repository.Setup(r => r.GetFilteredAsync(filter, false)).ReturnsAsync(transactions);

        var result = await _sut.GetFilteredAsync(filter, includeDeleted: false);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
    }
}
