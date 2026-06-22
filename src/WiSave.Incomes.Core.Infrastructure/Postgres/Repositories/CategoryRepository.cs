using WiSave.Incomes.Core.Application.Categories;
using WiSave.Incomes.Core.Infrastructure.Postgres.Entities;

namespace WiSave.Incomes.Core.Infrastructure.Postgres.Repositories;

public sealed class CategoryRepository(IncomesDbContext db) : ICategoryRepository
{
    public async Task CreateAsync(
        Guid id,
        Guid userId,
        string name,
        int sortOrder,
        CancellationToken ct = default)
    {
        db.Categories.Add(new CategoryEntity
        {
            Id = id,
            UserId = userId,
            Name = name,
            SortOrder = sortOrder
        });

        await db.SaveChangesAsync(ct);
    }
}
