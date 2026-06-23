using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Projections.Postgres;
using WiSave.Incomes.Projections.Postgres.Entities;

namespace WiSave.Incomes.Projections.EventHandlers;

public sealed class CategoryCreatedEventHandler(ProjectionsDbContext db)
{
    public async Task Handle(CategoryCreated @event, CancellationToken ct = default)
    {
        db.Categories.Add(new CategoryEntity
        {
            Id = @event.Id,
            UserId = @event.UserId,
            Name = @event.Name,
            SortOrder = @event.SortOrder,
            Subcategories = []
        });

        await db.SaveChangesAsync(ct);
    }
}
