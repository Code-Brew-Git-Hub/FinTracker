using FinTracker.Application.Abstractions.Repositories;
using FinTracker.Application.Services;
using FinTracker.Domain.Dtos.Analytics;
using FinTracker.Domain.Enums;
using FinTracker.Domain.Models;
using Moq;

namespace Application.UnitTests.Services;

public class AnalyticsServiceTests
{
    private readonly Mock<IAnalyticsRepository> _repository = new();
    private readonly AnalyticsService _sut;

    public AnalyticsServiceTests()
    {
        _sut = new AnalyticsService(_repository.Object);
    }

    [Fact]
    public async Task GetSummaryAsync_CalculatesIncomeExpenseBalance()
    {
        var food = new Category { Id = Guid.NewGuid(), Name = "Food" };
        var salary = new Category { Id = Guid.NewGuid(), Name = "Salary" };

        _repository.Setup(r => r.GetFilteredAsync(It.IsAny<AnalyticsFilterDto>()))
            .ReturnsAsync(
            [
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = 1000,
                    Type = TransactionType.Income,
                    Category = salary,
                    CategoryId = salary.Id,
                    TransactionTags = []
                },
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = -300,
                    Type = TransactionType.Expense,
                    Category = food,
                    CategoryId = food.Id,
                    TransactionTags = []
                },
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = -200,
                    Type = TransactionType.Expense,
                    Category = food,
                    CategoryId = food.Id,
                    TransactionTags = []
                }
            ]);

        var result = await _sut.GetSummaryAsync(new AnalyticsFilterDto());

        Assert.Equal(1000, result.TotalIncome);
        Assert.Equal(500, result.TotalExpense);
        Assert.Equal(500, result.Balance);
    }

    [Fact]
    public async Task GetByCategoryAsync_CalculatesPercentages()
    {
        var food = new Category { Id = Guid.NewGuid(), Name = "Food" };
        var transport = new Category { Id = Guid.NewGuid(), Name = "Transport" };

        _repository.Setup(r => r.GetFilteredAsync(It.IsAny<AnalyticsFilterDto>()))
            .ReturnsAsync(
            [
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = -300,
                    Type = TransactionType.Expense,
                    Category = food,
                    CategoryId = food.Id,
                    TransactionTags = []
                },
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = -100,
                    Type = TransactionType.Expense,
                    Category = transport,
                    CategoryId = transport.Id,
                    TransactionTags = []
                }
            ]);

        var result = await _sut.GetByCategoryAsync(new AnalyticsFilterDto());

        var foodStat = result.First(x => x.Category.Name == "Food");
        var transportStat = result.First(x => x.Category.Name == "Transport");

        Assert.Equal(300, foodStat.Total);
        Assert.Equal(75, foodStat.Percent);
        Assert.Equal(100, transportStat.Total);
        Assert.Equal(25, transportStat.Percent);
    }

    [Fact]
    public async Task GetByTimeAsync_GroupsByDay()
    {
        var category = new Category { Id = Guid.NewGuid(), Name = "Food" };
        var day1 = new DateTime(2025, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var day2 = new DateTime(2025, 6, 2, 12, 0, 0, DateTimeKind.Utc);

        _repository.Setup(r => r.GetFilteredAsync(It.IsAny<AnalyticsFilterDto>()))
            .ReturnsAsync(
            [
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = 1000,
                    Type = TransactionType.Income,
                    DateUtc = day1,
                    Category = category,
                    CategoryId = category.Id,
                    TransactionTags = []
                },
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = -200,
                    Type = TransactionType.Expense,
                    DateUtc = day1,
                    Category = category,
                    CategoryId = category.Id,
                    TransactionTags = []
                },
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = -50,
                    Type = TransactionType.Expense,
                    DateUtc = day2,
                    Category = category,
                    CategoryId = category.Id,
                    TransactionTags = []
                }
            ]);

        var result = await _sut.GetByTimeAsync(new AnalyticsFilterDto(), TimeGrouping.Day);

        Assert.Equal(2, result.Count);
        Assert.Equal("2025-06-01", result[0].Period);
        Assert.Equal(1000, result[0].TotalIncome);
        Assert.Equal(200, result[0].TotalExpense);
        Assert.Equal(800, result[0].Balance);
        Assert.Equal("2025-06-02", result[1].Period);
    }

    [Fact]
    public async Task GetByTagAsync_GroupsByTag()
    {
        var tag = new Tag { Id = Guid.NewGuid(), Name = "work" };
        var category = new Category { Id = Guid.NewGuid(), Name = "Food" };

        _repository.Setup(r => r.GetFilteredAsync(It.IsAny<AnalyticsFilterDto>()))
            .ReturnsAsync(
            [
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = -100,
                    Type = TransactionType.Expense,
                    Category = category,
                    CategoryId = category.Id,
                    TransactionTags =
                    [
                        new TransactionTag { TagId = tag.Id, Tag = tag, TransactionId = Guid.NewGuid() }
                    ]
                },
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    Amount = -50,
                    Type = TransactionType.Expense,
                    Category = category,
                    CategoryId = category.Id,
                    TransactionTags =
                    [
                        new TransactionTag { TagId = tag.Id, Tag = tag, TransactionId = Guid.NewGuid() }
                    ]
                }
            ]);

        var result = await _sut.GetByTagAsync(new AnalyticsFilterDto());

        Assert.Single(result);
        Assert.Equal("work", result[0].Tag.Name);
        Assert.Equal(150, result[0].Total);
        Assert.Equal(2, result[0].Count);
    }
}
