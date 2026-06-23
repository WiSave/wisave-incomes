using WiSave.Framework.Application;
using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Contracts.Models;
using WiSave.Incomes.Core.Application.Abstractions;
using WiSave.Incomes.Core.Domain.Incomes;

namespace WiSave.Incomes.Core.Application.Incomes.CommandHandlers;

public sealed class CreateIncomeCommandHandler(
    IAggregateRepository<Income, IncomeId> repository,
    IEventPublisher eventPublisher)
{
    public async Task Handle(CreateIncomeCommand command, CancellationToken ct = default)
    {
        var income = Income.Create(
            new IncomeId(Guid.NewGuid()),
            command.Amount,
            command.IncomeDate,
            command.Name,
            command.Description,
            command.UserId,
            command.Tags,
            command.CategoryId,
            command.SubcategoryId);

        var events = income.GetUncommittedEvents()
            .OfType<IncomeCreated>()
            .ToArray();

        await repository.SaveAsync(income, ct);

        foreach (var @event in events)
        {
            await eventPublisher.PublishAsync(@event, ct);
        }
    }
}
