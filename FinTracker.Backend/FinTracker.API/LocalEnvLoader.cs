namespace FinTracker.API;

internal static class LocalEnvLoader
{
    public static void LoadIfPresent()
    {
        var envPath = FindEnvFile();
        if (envPath is null)
            return;

        DotNetEnv.Env.Load(envPath);
    }

    private static string? FindEnvFile()
    {
        var searched = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var root in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            var directory = new DirectoryInfo(root);
            for (var depth = 0; depth < 10 && directory is not null; depth++, directory = directory.Parent)
            {
                var candidate = Path.Combine(directory.FullName, ".env");
                if (!searched.Add(candidate))
                    continue;

                if (File.Exists(candidate))
                    return candidate;
            }
        }

        return null;
    }
}
