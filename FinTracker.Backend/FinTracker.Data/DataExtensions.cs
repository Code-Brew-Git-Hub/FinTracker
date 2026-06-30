using FinTracker.Application.Abstractions.Repositories;
using FinTracker.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinTracker.Data;

public static class DataExtensions
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

}
