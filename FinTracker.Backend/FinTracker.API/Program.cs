using FinTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTracker.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        LocalEnvLoader.LoadIfPresent();

        var builder = WebApplication.CreateBuilder(args);

        builder.ConfigureBuilder();

        var app = builder.Build();

        // Применение миграций
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
            await ImportPresetSeedData.SeedAsync(db);
        }

        app.ConfigureApp();

        app.Run();
    }
}
