using FinTracker.Application.Services;
using FinTracker.Domain.Dtos.TransactionLinks;
using FinTracker.Domain.Enums;
using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Models;
using MapsterMapper;
using Moq;

namespace Application.UnitTests.Services;

public class LinkServiceTests
{
    private readonly Mock<ILinkRepository> _linkRepository = new();
    private readonly Mock<ITransactionRepository> _transactionRepository = new();
    private readonly IMapper _mapper = TestMapperFactory.Create();
    private readonly LinkService _sut;

    public LinkServiceTests()
    {
        _sut = new LinkService(_linkRepository.Object, _transactionRepository.Object, _mapper);
    }

    [Fact]
    public async Task CreateAsync_WithLessThanTwoTransactions_Throws()
    {
        var dto = new CreateTransactionLinkDto
        {
            Type = TransactionLinkType.Transfer,
            TransactionIds = [Guid.NewGuid()]
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(dto));

        Assert.Contains("минимум две", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_WithTwoTransactions_CreatesLink()
    {
        var tx1 = Guid.NewGuid();
        var tx2 = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        _transactionRepository.Setup(r => r.GetByIdAsync(tx1))
            .ReturnsAsync(new Transaction { Id = tx1, CategoryId = categoryId });
        _transactionRepository.Setup(r => r.GetByIdAsync(tx2))
            .ReturnsAsync(new Transaction { Id = tx2, CategoryId = categoryId });
        _linkRepository.Setup(r => r.AddAsync(It.IsAny<TransactionLink>())).Returns(Task.CompletedTask);
        _linkRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _sut.CreateAsync(new CreateTransactionLinkDto
        {
            Type = TransactionLinkType.Transfer,
            TransactionIds = [tx1, tx2]
        });

        Assert.Equal("Transfer", result.Type);
        _linkRepository.Verify(r => r.AddAsync(It.Is<TransactionLink>(l => l.Entries.Count == 2)), Times.Once);
    }

    [Fact]
    public async Task AddTransactionAsync_WhenAlreadyInLink_Throws()
    {
        var linkId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var link = new TransactionLink
        {
            Id = linkId,
            Type = TransactionLinkType.Transfer,
            Entries =
            [
                new TransactionLinkEntry { TransactionLinkId = linkId, TransactionId = transactionId },
                new TransactionLinkEntry { TransactionLinkId = linkId, TransactionId = Guid.NewGuid() }
            ]
        };

        _linkRepository.Setup(r => r.GetByIdAsync(linkId)).ReturnsAsync(link);
        _transactionRepository.Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync(new Transaction { Id = transactionId, CategoryId = Guid.NewGuid() });

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.AddTransactionAsync(linkId, transactionId));

        Assert.Contains("already in this link", ex.Message);
    }

    [Fact]
    public async Task RemoveTransactionAsync_WhenOnlyTwoEntries_Throws()
    {
        var linkId = Guid.NewGuid();
        var tx1 = Guid.NewGuid();
        var tx2 = Guid.NewGuid();
        var link = new TransactionLink
        {
            Id = linkId,
            Type = TransactionLinkType.Transfer,
            Entries =
            [
                new TransactionLinkEntry { TransactionLinkId = linkId, TransactionId = tx1 },
                new TransactionLinkEntry { TransactionLinkId = linkId, TransactionId = tx2 }
            ]
        };

        _linkRepository.Setup(r => r.GetByIdWithEntriesAsync(linkId)).ReturnsAsync(link);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.RemoveTransactionAsync(linkId, tx1));

        Assert.Contains("минимум две", ex.Message);
    }

    [Fact]
    public async Task RemoveTransactionAsync_WhenMoreThanTwo_RemovesEntry()
    {
        var linkId = Guid.NewGuid();
        var tx1 = Guid.NewGuid();
        var tx2 = Guid.NewGuid();
        var tx3 = Guid.NewGuid();
        var link = new TransactionLink
        {
            Id = linkId,
            Type = TransactionLinkType.Transfer,
            Entries =
            [
                new TransactionLinkEntry { TransactionLinkId = linkId, TransactionId = tx1 },
                new TransactionLinkEntry { TransactionLinkId = linkId, TransactionId = tx2 },
                new TransactionLinkEntry { TransactionLinkId = linkId, TransactionId = tx3 }
            ]
        };

        _linkRepository.Setup(r => r.GetByIdWithEntriesAsync(linkId)).ReturnsAsync(link);
        _linkRepository.Setup(r => r.UpdateAsync(link)).Returns(Task.CompletedTask);
        _linkRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _sut.RemoveTransactionAsync(linkId, tx1);

        Assert.Equal(2, link.Entries.Count);
        Assert.DoesNotContain(link.Entries, e => e.TransactionId == tx1);
    }
}
