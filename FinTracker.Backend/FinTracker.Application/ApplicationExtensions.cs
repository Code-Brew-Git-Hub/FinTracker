using FinTracker.Application.Services;
using FinTracker.Domain.Interfaces.Services;
using FinTracker.Parser;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace FinTracker.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ITransactionService, TransactionService>();
        serviceCollection.AddScoped<IScopeService, ScopeService>();
        serviceCollection.AddScoped<ICategoryService, CategoryService>();
        serviceCollection.AddScoped<ITagService, TagService>();
        serviceCollection.AddScoped<IImportService, ImportService>();
        serviceCollection.AddScoped<IImportPresetService, ImportPresetService>();
        serviceCollection.AddScoped<IPositionService, PositionService>();
        serviceCollection.AddScoped<ILinkService, LinkService>();
        serviceCollection.AddScoped<IAnalyticsService, AnalyticsService>();

        return serviceCollection;
    }

    public static IServiceCollection AddParser(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<CsvParser>();
        serviceCollection.AddScoped<TransactionParser>();

        return serviceCollection;
    }

    public static IServiceCollection AddMapper(this IServiceCollection serviceCollection)
    {
        MappingConfig.Configure();
        serviceCollection.AddMapster();

        return serviceCollection;
    }
}