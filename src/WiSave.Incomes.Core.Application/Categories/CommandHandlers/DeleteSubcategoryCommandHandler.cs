using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Core.Application.Abstractions;

namespace WiSave.Incomes.Core.Application.Categories.CommandHandlers;

public sealed class DeleteSubcategoryCommandHandler(ICategoryRepository repository)
{
    public async Task Handle(DeleteSubcategory command, IEventPublisher eventPublisher, CancellationToken ct = default)
    {
        var deleted = await repository.DeleteSubcategoryAsync(
            command.CategoryId,
            command.Id,
            command.UserId,
            ct);

        if (!deleted)
        {
            return;
        }

        await eventPublisher.PublishAsync(
            new SubcategoryDeleted(command.CategoryId, command.Id, command.UserId),
            ct);
    }
}
