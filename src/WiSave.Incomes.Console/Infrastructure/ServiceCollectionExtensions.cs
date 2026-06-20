using Microsoft.Extensions.DependencyInjection;
using WiSave.Console.Infrastructure;
using WiSave.Incomes.Console.Operations;

namespace WiSave.Incomes.Console.Infrastructure;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIncomesConsole(this IServiceCollection services)
    {
        services.AddWiSaveConsole(
            options => options.Title = "WiSave Incomes Console",
            typeof(ServiceCollectionExtensions).Assembly);

        services.AddSingleton<IScopedDatabaseMigrator, CoreDatabaseMigrator>();
        services.AddSingleton<IScopedDatabaseMigrator, ProjectionsDatabaseMigrator>();
        services.AddSingleton<IDatabaseMigrationOperations, DatabaseMigrationOperations>();
        services.AddSingleton<IProjectionClearOperations, ProjectionClearOperations>();
        services.AddSingleton<IProjectionStorageResetClient, NpgsqlProjectionStorageResetClient>();

        return services;
    }
}
