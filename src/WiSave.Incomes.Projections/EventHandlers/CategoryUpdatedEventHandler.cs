using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Projections.Postgres;

namespace WiSave.Incomes.Projections.EventHandlers;

public sealed class CategoryUpdatedEventHandler(ProjectionsDbContext db)
{
    public async Task Handle(CategoryUpdated @event, CancellationToken ct = default)
    {
        var category = await db.Categories.FindAsync([@event.Id], ct);

        if (category is null || category.UserId != @event.UserId)
        {
            return;
        }

        category.Name = @event.Name;
        category.SortOrder = @event.SortOrder;

        await db.SaveChangesAsync(ct);
    }
}
