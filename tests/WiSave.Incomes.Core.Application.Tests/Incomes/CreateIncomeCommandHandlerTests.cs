using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Contracts.Models;
using WiSave.Incomes.Core.Application.Abstractions;
using WiSave.Incomes.Core.Application.Incomes.CommandHandlers;
using WiSave.Incomes.Core.Domain.Incomes;
using WiSave.Framework.Application;

namespace WiSave.Incomes.Core.Application.Tests.Incomes;

public class CreateIncomeCommandHandlerTests
{
    [Fact]
    public async Task Handle_creates_saves_and_publishes_income()
    {
        var operations = new List<string>();
        var repository = new CapturingIncomeRepository(operations);
        var publisher = new CapturingEventPublisher(operations);
        var handler = new CreateIncomeCommandHandler(repository, publisher);
        var command = new CreateIncomeCommand(
            new Money(2500m, Currency.PLN),
            new DateOnly(2026, 6, 18),
            "Salary",
            "June salary",
            Guid.Parse("018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234"),
            ["salary", "work"]);

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal(["save", "publish"], operations);
        Assert.NotNull(repository.Saved);
        var saved = repository.Saved;
        Assert.NotEqual(Guid.Empty, saved.Id.Value);
        Assert.Equal(command.Amount, saved.Amount);
        Assert.Equal(command.IncomeDate, saved.IncomeDate);
        Assert.Equal(command.Name, saved.Name);
        Assert.Equal(command.Description, saved.Description);
        Assert.Equal(command.UserId, saved.UserId);
        Assert.Equal(command.Tags, saved.Tags);

        var published = Assert.IsType<IncomeCreated>(Assert.Single(publisher.Published));
        Assert.Equal(saved.Id, published.Id);
        Assert.Equal(command.Amount, published.Amount);
        Assert.Equal(command.IncomeDate, published.IncomeDate);
        Assert.Equal(command.Name, published.Name);
        Assert.Equal(command.Description, published.Description);
        Assert.Equal(command.UserId, published.UserId);
        Assert.Equal(command.Tags, published.Tags);
    }

    private sealed class CapturingIncomeRepository(List<string> operations) : IAggregateRepository<Income, IncomeId>
    {
        public Income? Saved { get; private set; }

        public Task<Income?> LoadAsync(IncomeId id, CancellationToken ct = default)
        {
            return Task.FromResult<Income?>(null);
        }

        public Task SaveAsync(Income aggregate, CancellationToken ct = default)
        {
            operations.Add("save");
            Saved = aggregate;
            return Task.CompletedTask;
        }
    }

    private sealed class CapturingEventPublisher(List<string> operations) : IEventPublisher
    {
        public List<object> Published { get; } = [];

        public Task PublishAsync(object @event, CancellationToken ct = default)
        {
            operations.Add("publish");
            Published.Add(@event);
            return Task.CompletedTask;
        }
    }
}
