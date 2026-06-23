using Microsoft.EntityFrameworkCore;

namespace FinTracker.Data;

public static class DatabaseMigrationHelper
{
    private const string CurrentInitMigration = "20260617183151_Init";
    private const string LegacyInitMigration = "20260520162946_Init";
    private const string ProductVersion = "10.0.6";

    public static async Task MigrateWithLegacyBaselineAsync(AppDbContext db)
    {
        await BaselineLegacyDatabaseAsync(db);
        await db.Database.MigrateAsync();
    }

    private static async Task BaselineLegacyDatabaseAsync(AppDbContext db)
    {
        if (!await TableExistsAsync(db, "Categories"))
            return;

        var applied = (await db.Database.GetAppliedMigrationsAsync()).ToList();
        if (applied.Contains(CurrentInitMigration))
            return;

        if (applied.Contains(LegacyInitMigration))
        {
            await db.Database.ExecuteSqlRawAsync(
                """
                DELETE FROM "__EFMigrationsHistory"
                WHERE "MigrationId" = '20260520162946_Init'
                """);
        }

        if (!await TableExistsAsync(db, "ImportPresets"))
        {
            await db.Database.ExecuteSqlRawAsync(
                """
                CREATE TABLE "ImportPresets" (
                    "Id" uuid NOT NULL,
                    "Name" character varying(100) NOT NULL,
                    "ParseOptionsJson" text NOT NULL,
                    "MatchHeadersJson" text NOT NULL,
                    "IsActive" boolean NOT NULL,
                    CONSTRAINT "PK_ImportPresets" PRIMARY KEY ("Id")
                );

                CREATE UNIQUE INDEX "IX_ImportPresets_Name" ON "ImportPresets" ("Name");
                """);
        }

        await db.Database.ExecuteSqlRawAsync(
            $"""
            INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            VALUES ('{CurrentInitMigration}', '{ProductVersion}')
            ON CONFLICT ("MigrationId") DO NOTHING
            """);
    }

    private static Task<bool> TableExistsAsync(AppDbContext db, string tableName) =>
        db.Database.SqlQuery<bool>(
                $"""
                SELECT to_regclass('public."{tableName}"') IS NOT NULL AS "Value"
                """)
            .SingleAsync();
}
