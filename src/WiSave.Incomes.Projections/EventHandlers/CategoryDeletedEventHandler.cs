using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Projections.Postgres;

namespace WiSave.Incomes.Projections.EventHandlers;

public sealed class CategoryDeletedEventHandler(ProjectionsDbContext db)
{
    public async Task Handle(CategoryDeleted @event, CancellationToken ct = default)
    {
        var category = await db.Categories.FindAsync([@event.Id], ct);

        if (category is null || category.UserId != @event.UserId)
        {
            return;
        }

        db.Categories.Remove(category);
        await db.SaveChangesAsync(ct);
    }
}
