using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WiSave.Incomes.Core.Application.Categories;
using WiSave.Incomes.Core.Infrastructure.Postgres.Repositories;

namespace WiSave.Incomes.Core.Infrastructure.Postgres;

public static class PostgresExtensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<IncomesDbContext>(opts => opts.UseNpgsql(connectionString));
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
