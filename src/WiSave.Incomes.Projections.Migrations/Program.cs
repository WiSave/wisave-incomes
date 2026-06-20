using Npgsql;

namespace WiSave.Incomes.Projections.Migrations;

public static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            var rebuild = args.Contains("--rebuild");

            var connectionString = args.FirstOrDefault(a => !a.StartsWith("--", StringComparison.Ordinal));
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Incomes");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.Error.WriteLine(
                    "Connection string not provided. Pass it as the first argument or set ConnectionStrings__Incomes.");
                return 1;
            }

            if (rebuild)
            {
                Console.WriteLine("Rebuilding projections schema...");
                DropProjectionsSchema(connectionString);
            }

            DbMigrator.Run(connectionString);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error when running migration: {ex.Message}");
            Console.Error.WriteLine(ex.StackTrace);
            return 1;
        }
    }

    private static void DropProjectionsSchema(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DROP SCHEMA IF EXISTS projections CASCADE;";
        command.ExecuteNonQuery();

        Console.WriteLine("Dropped projections schema.");
    }
}
