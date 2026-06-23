using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Projections.Postgres;
using WiSave.Incomes.Projections.Postgres.Entities;

namespace WiSave.Incomes.Projections.EventHandlers;

public sealed class SubcategoryCreatedEventHandler(ProjectionsDbContext db)
{
    public async Task Handle(SubcategoryCreated @event, CancellationToken ct = default)
    {
        var category = await db.Categories.FindAsync([@event.CategoryId], ct);

        if (category is null || category.UserId != @event.UserId)
        {
            return;
        }

        var subcategory = category.Subcategories.SingleOrDefault(x => x.Id == @event.Id);

        if (subcategory is null)
        {
            category.Subcategories.Add(new SubcategoryEntity
            {
                Id = @event.Id,
                Name = @event.Name,
                SortOrder = @event.SortOrder
            });
        }
        else
        {
            subcategory.Name = @event.Name;
            subcategory.SortOrder = @event.SortOrder;
        }

        await db.SaveChangesAsync(ct);
    }
}
