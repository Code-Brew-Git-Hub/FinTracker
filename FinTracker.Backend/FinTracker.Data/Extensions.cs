using FinTracker.Domain.Interfaces.Repositories;
using FinTracker.Domain.Interfaces.Services;
using FinTracker.Data.Repositories;
using FinTracker.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FinTracker.Parser;
using Mapster;

namespace FinTracker.Data;

public static class Extensions
{
    public static IServiceCollection AddContext(this IServiceCollection serviceCollection)
    {
        var dbHost = RequireEnv("DB_HOST", "Переменная DB_HOST (Хост БД) не задана");
        var dbPort = RequireEnv("DB_PORT", "Переменная DB_PORT (Порт БД) не задана");
        var dbName = RequireEnv("DB_NAME", "Переменная DB_NAME (Имя базы данных) не задана");
        var dbUser = RequireEnv("DB_USER", "Переменная DB_USER (Пользователь БД) не задана");
        var dbPassword = RequireEnv("DB_PASSWORD", "Переменная DB_PASSWORD (Пароль БД) не задана");

        serviceCollection.AddDbContext<AppDbContext>(x =>
        {
            x.UseNpgsql($"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}");
        });

        return serviceCollection;
    }

    private static string RequireEnv(string name, string errorMessage)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException(errorMessage);

        return value;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ITransactionRepository, TransactionRepository>();
        serviceCollection.AddScoped<IScopeRepository, ScopeRepository>();
        serviceCollection.AddScoped<ICategoryRepository, CategoryRepository>();
        serviceCollection.AddScoped<ITagRepository, TagRepository>();
        serviceCollection.AddScoped<IPositionRepository, PositionRepository>();
        serviceCollection.AddScoped<ILinkRepository, LinkRepository>();
        serviceCollection.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
        serviceCollection.AddScoped<IImportPresetRepository, ImportPresetRepository>();

        return serviceCollection;
    }

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
