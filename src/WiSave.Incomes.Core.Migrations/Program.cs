namespace WiSave.Incomes.Core.Migrations;

public static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            var connectionString = args.FirstOrDefault(a => !a.StartsWith("--", StringComparison.Ordinal));
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__General");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.Error.WriteLine(
                    "Connection string not provided. Pass it as the first argument or set ConnectionStrings__General.");
                return 1;
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
}
