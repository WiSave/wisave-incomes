using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Projections.Postgres;

namespace WiSave.Incomes.Projections.EventHandlers;

public sealed class SubcategoryUpdatedEventHandler(ProjectionsDbContext db)
{
    public async Task Handle(SubcategoryUpdated @event, CancellationToken ct = default)
    {
        var category = await db.Categories.FindAsync([@event.CategoryId], ct);

        if (category is null)
        {
            throw new InvalidOperationException(
                $"Cannot apply {nameof(SubcategoryUpdated)} because category projection '{@event.CategoryId}' was not found.");
        }

        if (category.UserId != @event.UserId)
        {
            throw new InvalidOperationException(
                $"Cannot apply {nameof(SubcategoryUpdated)} because category projection '{@event.CategoryId}' belongs to a different user.");
        }

        var subcategory = category.Subcategories.SingleOrDefault(x => x.Id == @event.Id);

        if (subcategory is null)
        {
            throw new InvalidOperationException(
                $"Cannot apply {nameof(SubcategoryUpdated)} because subcategory projection '{@event.Id}' was not found.");
        }

        subcategory.Name = @event.Name;
        subcategory.SortOrder = @event.SortOrder;

        await db.SaveChangesAsync(ct);
    }
}
