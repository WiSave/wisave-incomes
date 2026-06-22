using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Core.Application.Abstractions;

namespace WiSave.Incomes.Core.Application.Categories.CommandHandlers;

public sealed class CreateCategoryCommandHandler(
    ICategoryRepository repository,
    IEventPublisher eventPublisher)
{
    public async Task Handle(CreateCategory command, CancellationToken ct = default)
    {
        var sortOrder = command.SortOrder ?? 0;

        await repository.CreateAsync(
            command.Id,
            command.UserId,
            command.Name,
            sortOrder,
            ct);

        await eventPublisher.PublishAsync(
            new CategoryCreated(
                command.Id,
                command.UserId,
                command.Name,
                sortOrder),
            ct);
    }
}
