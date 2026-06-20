namespace WiSave.Incomes.Core.Application.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync(object @event, CancellationToken ct = default);
}
