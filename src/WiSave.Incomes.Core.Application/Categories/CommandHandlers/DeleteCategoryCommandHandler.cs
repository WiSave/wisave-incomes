using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Core.Application.Abstractions;

namespace WiSave.Incomes.Core.Application.Categories.CommandHandlers;

public sealed class DeleteCategoryCommandHandler(ICategoryRepository repository)
{
    public async Task Handle(DeleteCategory command, IEventPublisher eventPublisher, CancellationToken ct = default)
    {
        var deleted = await repository.DeleteAsync(command.Id, command.UserId, ct);

        if (!deleted)
        {
            return;
        }

        await eventPublisher.PublishAsync(
            new CategoryDeleted(command.Id, command.UserId),
            ct);
    }
}
