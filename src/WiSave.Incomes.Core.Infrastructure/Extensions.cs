using Microsoft.Extensions.DependencyInjection;
using WiSave.Framework.EventSourcing.Marten;
using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Core.Domain;
using WiSave.Incomes.Core.Infrastructure.Identity;
using WiSave.Incomes.Core.Infrastructure.Postgres;

namespace WiSave.Incomes.Core.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddIncomesInfrastructure(
        this IServiceCollection services,
        string postgresConnectionString,
        string eventStoreConnectionString)
    {
        services.AddPostgres(postgresConnectionString);
        services.AddMartenEventStore(
            eventStoreConnectionString,
            [typeof(DomainAssemblyMarker).Assembly, typeof(IncomeCreated).Assembly],
            type => type.Namespace == typeof(IncomeCreated).Namespace
                || type.Name.EndsWith("Event", StringComparison.Ordinal));

        services.AddIdentity();

        return services;
    }
}
