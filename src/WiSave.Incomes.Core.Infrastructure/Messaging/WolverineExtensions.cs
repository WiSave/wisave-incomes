using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WiSave.Incomes.Core.Application.Abstractions;
using WiSave.Incomes.Core.Infrastructure.Postgres;
using Wolverine;
using Wolverine.RabbitMQ;
using Wolverine.Transports;

namespace WiSave.Incomes.Core.Infrastructure.Messaging;

public static class WolverineExtensions
{
    public static IServiceCollection AddIncomesEventPublishing(this IServiceCollection services)
    {
        services.AddScoped<IEventPublisher, WolverineEventPublisher>();
        return services;
    }

    public static IHostApplicationBuilder AddIncomesWolverine(this IHostApplicationBuilder builder, Action<WolverineOptions>? configure = null)
    {
        var rabbitMq = RabbitMqSettings.FromConfiguration(builder.Configuration);

        builder.UseWolverine(options =>
        {
            options.CodeGeneration.AlwaysUseServiceLocationFor<DbContextOptions<IncomesDbContext>>();

            options.UseRabbitMq(rabbit =>
                {
                    rabbit.HostName = rabbitMq.Host;
                    rabbit.VirtualHost = rabbitMq.VirtualHost;
                    rabbit.UserName = rabbitMq.Username;
                    rabbit.Password = rabbitMq.Password;
                })
                .EnableEnhancedDeadLettering()
                .AutoProvision()
                .UseConventionalRouting(NamingSource.FromHandlerType);

            configure?.Invoke(options);
        });

        return builder;
    }

    public static WolverineOptions IncludeHandlerAssembly<T>(this WolverineOptions options)
    {
        options.Discovery.IncludeAssembly(typeof(T).Assembly);
        return options;
    }

    public static WolverineOptions IncludeHandlerAssembly(
        this WolverineOptions options,
        Assembly assembly)
    {
        options.Discovery.IncludeAssembly(assembly);
        return options;
    }
}
