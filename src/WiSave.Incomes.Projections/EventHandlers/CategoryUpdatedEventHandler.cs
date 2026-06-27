using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Projections.Postgres;

namespace WiSave.Incomes.Projections.EventHandlers;

public sealed class CategoryUpdatedEventHandler(ProjectionsDbContext db)
{
    public async Task Handle(CategoryUpdated @event, CancellationToken ct = default)
    {
        var category = await db.Categories.FindAsync([@event.Id], ct);

        if (category is null)
        {
            throw new InvalidOperationException(
                $"Cannot apply {nameof(CategoryUpdated)} because category projection '{@event.Id}' was not found.");
        }

        if (category.UserId != @event.UserId)
        {
            throw new InvalidOperationException(
                $"Cannot apply {nameof(CategoryUpdated)} because category projection '{@event.Id}' belongs to a different user.");
        }

        category.Name = @event.Name;
        category.SortOrder = @event.SortOrder;

        await db.SaveChangesAsync(ct);
    }
}
