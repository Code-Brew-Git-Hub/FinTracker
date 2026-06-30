using FinTracker.Application.Services;
using FinTracker.Domain.Dtos.Positions;
using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Models;
using MapsterMapper;
using Moq;

namespace Application.UnitTests.Services;

public class PositionServiceTests
{
    private readonly Mock<IPositionRepository> _positionRepository = new();
    private readonly Mock<ITransactionRepository> _transactionRepository = new();
    private readonly IMapper _mapper = TestMapperFactory.Create();
    private readonly PositionService _sut;

    public PositionServiceTests()
    {
        _sut = new PositionService(_positionRepository.Object, _transactionRepository.Object, _mapper);
    }

    [Fact]
    public async Task CreateAsync_CreatesPosition()
    {
        var transactionId = Guid.NewGuid();
        var dto = new CreatePositionDto { Name = "Bread", Amount = 50 };

        _transactionRepository.Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync(new Transaction { Id = transactionId, CategoryId = Guid.NewGuid() });
        _positionRepository.Setup(r => r.AddAsync(It.IsAny<Position>())).Returns(Task.CompletedTask);
        _positionRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(transactionId, dto);

        Assert.Equal("Bread", result.Name);
        Assert.Equal(50, result.Amount);
        _positionRepository.Verify(r => r.AddAsync(It.Is<Position>(p =>
            p.TransactionId == transactionId && p.Name == "Bread")), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenWrongTransaction_ThrowsArgumentException()
    {
        var transactionId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        _positionRepository.Setup(r => r.GetByIdAsync(itemId))
            .ReturnsAsync(new Position
            {
                Id = itemId,
                TransactionId = Guid.NewGuid(),
                Name = "Bread",
                Amount = 50
            });

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.DeleteAsync(transactionId, itemId));

        Assert.Contains("does not belong", ex.Message);
    }

    [Fact]
    public async Task DeleteAsync_WhenBelongsToTransaction_Deletes()
    {
        var transactionId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        _positionRepository.Setup(r => r.GetByIdAsync(itemId))
            .ReturnsAsync(new Position
            {
                Id = itemId,
                TransactionId = transactionId,
                Name = "Bread",
                Amount = 50
            });
        _positionRepository.Setup(r => r.DeleteAsync(itemId)).Returns(Task.CompletedTask);
        _positionRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _sut.DeleteAsync(transactionId, itemId);

        _positionRepository.Verify(r => r.DeleteAsync(itemId), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPositions()
    {
        var transactionId = Guid.NewGuid();
        _transactionRepository.Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync(new Transaction { Id = transactionId, CategoryId = Guid.NewGuid() });
        _positionRepository.Setup(r => r.GetAllByTransactionIdAsync(transactionId))
            .ReturnsAsync(
            [
                new Position { Id = Guid.NewGuid(), TransactionId = transactionId, Name = "Bread", Amount = 50 }
            ]);

        var result = await _sut.GetAllAsync(transactionId);

        Assert.Single(result);
        Assert.Equal("Bread", result[0].Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFields()
    {
        var transactionId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var item = new Position
        {
            Id = itemId,
            TransactionId = transactionId,
            Name = "Bread",
            Amount = 50
        };

        _transactionRepository.Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync(new Transaction { Id = transactionId, CategoryId = Guid.NewGuid() });
        _positionRepository.Setup(r => r.GetByIdAsync(itemId)).ReturnsAsync(item);
        _positionRepository.Setup(r => r.UpdateAsync(item)).Returns(Task.CompletedTask);
        _positionRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _sut.UpdateAsync(transactionId, itemId, new UpdatePositionDto
        {
            Name = "Milk",
            Amount = 80,
            CategoryId = categoryId
        });

        Assert.Equal("Milk", result.Name);
        Assert.Equal(80, result.Amount);
        Assert.Equal(categoryId, item.CategoryId);
    }
}
