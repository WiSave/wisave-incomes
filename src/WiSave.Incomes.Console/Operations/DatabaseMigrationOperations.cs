using System.Text;
using Microsoft.Extensions.Configuration;
using WiSave.Console.Shell;

namespace WiSave.Incomes.Console.Operations;

internal interface IDatabaseMigrationOperations
{
    Task<string> RunAsync(string? connectionString, CancellationToken ct);
}

internal sealed class DatabaseMigrationOperations(
    IConfiguration configuration,
    IConsoleOutput consoleOutput,
    IEnumerable<IScopedDatabaseMigrator> migrators) : IDatabaseMigrationOperations
{
    public async Task<string> RunAsync(string? connectionString, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var results = new List<MigrationRunResult>();
        foreach (var migrator in migrators)
        {
            var effectiveConnectionString = ResolveConnectionString(connectionString, migrator.ConnectionStringName);
            consoleOutput.WriteLine($"Applying {migrator.Scope} migrations...");
            results.Add(await migrator.RunAsync(effectiveConnectionString, ct));
        }

        return FormatSuccessMessage(results);
    }

    private static string? NormalizeConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString;
        }

        var trimmed = connectionString.Trim();
        if (trimmed.Length >= 2 &&
            ((trimmed[0] == '"' && trimmed[^1] == '"') ||
             (trimmed[0] == '\'' && trimmed[^1] == '\'')))
        {
            return trimmed[1..^1];
        }

        return trimmed;
    }

    private string ResolveConnectionString(string? overrideConnectionString, string connectionStringName)
    {
        var effectiveConnectionString = NormalizeConnectionString(overrideConnectionString);
        if (string.IsNullOrWhiteSpace(effectiveConnectionString))
        {
            effectiveConnectionString = NormalizeConnectionString(configuration.GetConnectionString(connectionStringName));
        }

        if (string.IsNullOrWhiteSpace(effectiveConnectionString))
        {
            throw new InvalidOperationException(
                $"Postgres connection string '{connectionStringName}' was not configured. Set ConnectionStrings__{connectionStringName} or appsettings.json.");
        }

        return effectiveConnectionString;
    }

    private static string FormatSuccessMessage(IReadOnlyList<MigrationRunResult> results)
    {
        var builder = new StringBuilder("Incomes database migrations applied.");

        foreach (var result in results)
        {
            builder.AppendLine();
            builder.Append(result.Scope);
            builder.Append(':');

            if (result.AppliedMigrations.Count == 0)
            {
                builder.AppendLine();
                builder.Append("- no new migrations");
                continue;
            }

            foreach (var migration in result.AppliedMigrations)
            {
                builder.AppendLine();
                builder.Append("- ");
                builder.Append(FormatMigrationName(migration));
            }
        }

        return builder.ToString();
    }

    private static string FormatMigrationName(string migrationName)
        => migrationName.Split(".Scripts.", StringSplitOptions.None) switch
        {
            [_, var formattedName] => formattedName,
            _ => migrationName
        };
}
