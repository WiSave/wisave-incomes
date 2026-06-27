using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Core.Application.Abstractions;

namespace WiSave.Incomes.Core.Application.Categories.CommandHandlers;

public sealed class UpdateSubcategoryCommandHandler(ICategoryRepository repository)
{
    public async Task Handle(UpdateSubcategory command, IEventPublisher eventPublisher, CancellationToken ct = default)
    {
        var sortOrder = command.SortOrder ?? 0;
        var updated = await repository.UpdateSubcategoryAsync(
            command.CategoryId,
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
            new SubcategoryUpdated(
                command.CategoryId,
                command.Id,
                command.UserId,
                command.Name,
                sortOrder),
            ct);
    }
}
