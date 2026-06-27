using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Projections.Postgres;

namespace WiSave.Incomes.Projections.EventHandlers;

public sealed class CategoryDeletedEventHandler(ProjectionsDbContext db)
{
    public async Task Handle(CategoryDeleted @event, CancellationToken ct = default)
    {
        var category = await db.Categories.FindAsync([@event.Id], ct);

        if (category is null)
        {
            throw new InvalidOperationException(
                $"Cannot apply {nameof(CategoryDeleted)} because category projection '{@event.Id}' was not found.");
        }

        if (category.UserId != @event.UserId)
        {
            throw new InvalidOperationException(
                $"Cannot apply {nameof(CategoryDeleted)} because category projection '{@event.Id}' belongs to a different user.");
        }

        db.Categories.Remove(category);
        await db.SaveChangesAsync(ct);
    }
}
