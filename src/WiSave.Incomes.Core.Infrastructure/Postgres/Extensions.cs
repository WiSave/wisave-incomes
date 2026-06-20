using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WiSave.Incomes.Core.Infrastructure.Postgres;

public static class PostgresExtensions
{
    public static IServiceCollection AddPostgres(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<IncomesDbContext>(opts => opts.UseNpgsql(connectionString));

        return services;
    }
}
