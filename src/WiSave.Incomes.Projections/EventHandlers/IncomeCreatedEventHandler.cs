using WiSave.Incomes.Contracts.Events;

namespace WiSave.Incomes.Projections.EventHandlers;

public sealed class IncomeCreatedEventHandler
{
    public Task Handle(IncomeCreated @event, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }
}
