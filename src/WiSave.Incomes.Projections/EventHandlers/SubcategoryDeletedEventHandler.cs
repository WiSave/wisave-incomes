using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Projections.Postgres;

namespace WiSave.Incomes.Projections.EventHandlers;

public sealed class SubcategoryDeletedEventHandler(ProjectionsDbContext db)
{
    public async Task Handle(SubcategoryDeleted @event, CancellationToken ct = default)
    {
        var category = await db.Categories.FindAsync([@event.CategoryId], ct);

        if (category is null)
        {
            throw new InvalidOperationException(
                $"Cannot apply {nameof(SubcategoryDeleted)} because category projection '{@event.CategoryId}' was not found.");
        }

        if (category.UserId != @event.UserId)
        {
            throw new InvalidOperationException(
                $"Cannot apply {nameof(SubcategoryDeleted)} because category projection '{@event.CategoryId}' belongs to a different user.");
        }

        var removed = category.Subcategories.RemoveAll(x => x.Id == @event.Id);

        if (removed == 0)
        {
            throw new InvalidOperationException(
                $"Cannot apply {nameof(SubcategoryDeleted)} because subcategory projection '{@event.Id}' was not found.");
        }

        await db.SaveChangesAsync(ct);
    }
}
