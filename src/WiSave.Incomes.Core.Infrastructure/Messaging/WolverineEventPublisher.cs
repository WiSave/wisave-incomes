using WiSave.Incomes.Core.Application.Abstractions;
using Wolverine;

namespace WiSave.Incomes.Core.Infrastructure.Messaging;

public sealed class WolverineEventPublisher(IMessageBus bus) : IEventPublisher
{
    public async Task PublishAsync(object @event, CancellationToken ct = default)
    {
        await bus.PublishAsync(@event);
    }
}
