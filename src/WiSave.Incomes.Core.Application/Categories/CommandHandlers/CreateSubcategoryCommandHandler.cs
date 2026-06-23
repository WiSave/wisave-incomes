using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Core.Application.Abstractions;

namespace WiSave.Incomes.Core.Application.Categories.CommandHandlers;

public sealed class CreateSubcategoryCommandHandler(ICategoryRepository repository)
{
    public async Task Handle(CreateSubcategory command, IEventPublisher eventPublisher, CancellationToken ct = default)
    {
        var sortOrder = command.SortOrder ?? 0;
        var created = await repository.CreateSubcategoryAsync(
            command.Id,
            command.CategoryId,
            command.UserId,
            command.Name,
            sortOrder,
            ct);

        if (!created)
        {
            return;
        }

        await eventPublisher.PublishAsync(
            new SubcategoryCreated(
                command.Id,
                command.CategoryId,
                command.UserId,
                command.Name,
                sortOrder),
            ct);
    }
}
