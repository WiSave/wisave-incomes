using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Core.Application.Abstractions;

namespace WiSave.Incomes.Core.Application.Categories.CommandHandlers;

public sealed class UpdateCategoryCommandHandler(ICategoryRepository repository)
{
    public async Task Handle(UpdateCategory command, IEventPublisher eventPublisher, CancellationToken ct = default)
    {
        var sortOrder = command.SortOrder ?? 0;
        var updated = await repository.UpdateAsync(
            command.Id,
            command.UserId,
            command.Name,
            sortOrder,
            ct);

        if (!updated)
        {
            return;
        }

        await eventPublisher.PublishAsync(
            new CategoryUpdated(
                command.Id,
                command.UserId,
                command.Name,
                sortOrder),
            ct);
    }
}
