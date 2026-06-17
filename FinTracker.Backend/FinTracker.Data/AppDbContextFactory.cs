using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FinTracker.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        LoadEnvFileIfPresent();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(BuildConnectionString());
        return new AppDbContext(optionsBuilder.Options);
    }

    private static string BuildConnectionString()
    {
        var host = GetEnv("DB_HOST", "localhost");
        var port = GetEnv("DB_PORT", "5432");
        var database = GetEnv("DB_NAME", "FinTrackerDb");
        var user = GetEnv("DB_USER", "postgres");
        var password = GetEnv("DB_PASSWORD", "postgres");

        return $"Host={host};Port={port};Database={database};Username={user};Password={password}";
    }

    private static string GetEnv(string name, string fallback)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? fallback : value;
    }

    private static void LoadEnvFileIfPresent()
    {
        foreach (var root in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            var directory = new DirectoryInfo(root);
            for (var depth = 0; depth < 10 && directory is not null; depth++, directory = directory.Parent)
            {
                var envPath = Path.Combine(directory.FullName, ".env");
                if (!File.Exists(envPath))
                    continue;

                foreach (var line in File.ReadAllLines(envPath))
                {
                    var trimmed = line.Trim();
                    if (trimmed.Length == 0 || trimmed.StartsWith('#'))
                        continue;

                    var separatorIndex = trimmed.IndexOf('=');
                    if (separatorIndex <= 0)
                        continue;

                    var key = trimmed[..separatorIndex].Trim();
                    var value = trimmed[(separatorIndex + 1)..].Trim().Trim('"');
                    Environment.SetEnvironmentVariable(key, value);
                }

                return;
            }
        }
    }
}
