using FinTracker.Domain.Dtos.Categories;
using FinTracker.Domain.Dtos.Scopes;
using FinTracker.Domain.Dtos.Tags;
using FinTracker.Domain.Dtos.Positions;
using FinTracker.Domain.Dtos.TransactionLinks;
using FinTracker.Domain.Dtos.Transactions;
using FinTracker.Domain.Models;
using Mapster;

namespace FinTracker.Data;

public static class MappingConfig
{
    public static void Configure()
    {
        // Category
        TypeAdapterConfig<Category, CategoryDto>.NewConfig();

        // Tag
        TypeAdapterConfig<Tag, TagDto>.NewConfig();

        // Scope
        TypeAdapterConfig<Scope, ScopeDto>.NewConfig();

        // Transaction → TransactionDto
        // Tags приходят через промежуточную таблицу TransactionTags
        TypeAdapterConfig<Transaction, TransactionDto>.NewConfig()
            .Map(dest => dest.Tags,
                 src => src.TransactionTags.Select(tt => tt.Tag).Adapt<List<TagDto>>())
            .Map(dest => dest.Type,
                src => src.Type.ToString());

        // CreateTransactionDto → Transaction
        TypeAdapterConfig<CreateTransactionDto, Transaction>.NewConfig()
            .Map(dest => dest.Id, _ => Guid.NewGuid())
            .Map(dest => dest.IsDeleted, _ => false)
            .Ignore(dest => dest.TransactionTags)
            .Ignore(dest => dest.Category)
            .Ignore(dest => dest.Scope);

        // TransactionItem
        TypeAdapterConfig<Position, PositionDto>.NewConfig();

        // TransactionLink — транзакции берём через Entries
        TypeAdapterConfig<TransactionLink, TransactionLinkDto>.NewConfig()
            .Map(dest => dest.Type,
                 src => src.Type.ToString())
            .Map(dest => dest.Transactions,
                 src => src.Entries.Select(e => e.Transaction).Adapt<List<TransactionDto>>());
    }
}