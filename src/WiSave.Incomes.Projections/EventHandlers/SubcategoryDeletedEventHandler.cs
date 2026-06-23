using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Projections.Postgres;

namespace WiSave.Incomes.Projections.EventHandlers;

public sealed class SubcategoryDeletedEventHandler(ProjectionsDbContext db)
{
    public async Task Handle(SubcategoryDeleted @event, CancellationToken ct = default)
    {
        var category = await db.Categories.FindAsync([@event.CategoryId], ct);

        if (category is null || category.UserId != @event.UserId)
        {
            return;
        }

        var removed = category.Subcategories.RemoveAll(x => x.Id == @event.Id);

        if (removed == 0)
        {
            return;
        }

        await db.SaveChangesAsync(ct);
    }
}
