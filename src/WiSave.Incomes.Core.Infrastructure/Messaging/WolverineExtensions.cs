using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WiSave.Incomes.Core.Application.Abstractions;
using Wolverine;
using Wolverine.RabbitMQ;

namespace WiSave.Incomes.Core.Infrastructure.Messaging;

public static class WolverineExtensions
{
    public static IHostApplicationBuilder AddIncomesWolverine(this IHostApplicationBuilder builder, Action<WolverineOptions>? configure = null)
    {
        var rabbitMq = RabbitMqSettings.FromConfiguration(builder.Configuration);

        builder.Services.AddScoped<IEventPublisher, WolverineEventPublisher>();

        builder.UseWolverine(options =>
        {
            options.UseRabbitMq(rabbit =>
                {
                    rabbit.HostName = rabbitMq.Host;
                    rabbit.VirtualHost = rabbitMq.VirtualHost;
                    rabbit.UserName = rabbitMq.Username;
                    rabbit.Password = rabbitMq.Password;
                })
                .EnableEnhancedDeadLettering()
                .AutoProvision()
                .UseConventionalRouting();

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
