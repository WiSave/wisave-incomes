using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WiSave.Incomes.Core.Application.Abstractions;
using WiSave.Incomes.Core.Infrastructure.Messaging;

namespace WiSave.Incomes.WebApi.Tests.Messaging;

public class WolverineRegistrationTests
{
    [Fact]
    public void AddIncomesWolverine_does_not_register_event_publisher()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.AddIncomesWolverine();

        Assert.DoesNotContain(builder.Services, descriptor => descriptor.ServiceType == typeof(IEventPublisher));
    }

    [Fact]
    public void AddIncomesEventPublishing_registers_event_publisher()
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddIncomesEventPublishing();

        Assert.Contains(
            builder.Services,
            descriptor =>
                descriptor.ServiceType == typeof(IEventPublisher) &&
                descriptor.ImplementationType == typeof(WolverineEventPublisher) &&
                descriptor.Lifetime == ServiceLifetime.Scoped);
    }
}
