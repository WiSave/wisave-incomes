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

    public async Task<bool> UpdateAsync(
        Guid id,
        Guid userId,
        string name,
        int sortOrder,
        CancellationToken ct = default)
    {
        var category = await db.Categories.FindAsync([id], ct);

        if (category is null || category.UserId != userId)
        {
            return false;
        }

        category.Name = name;
        category.SortOrder = sortOrder;

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken ct = default)
    {
        var category = await db.Categories.FindAsync([id], ct);

        if (category is null || category.UserId != userId)
        {
            return false;
        }

        db.Categories.Remove(category);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> CreateSubcategoryAsync(
        Guid id,
        Guid categoryId,
        Guid userId,
        string name,
        int sortOrder,
        CancellationToken ct = default)
    {
        var category = await db.Categories.FindAsync([categoryId], ct);

        if (category is null || category.UserId != userId)
        {
            return false;
        }

        db.Subcategories.Add(new SubcategoryEntity
        {
            Id = id,
            CategoryId = categoryId,
            Name = name,
            SortOrder = sortOrder
        });

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteSubcategoryAsync(
        Guid categoryId,
        Guid id,
        Guid userId,
        CancellationToken ct = default)
    {
        var category = await db.Categories.FindAsync([categoryId], ct);

        if (category is null || category.UserId != userId)
        {
            return false;
        }

        var subcategory = await db.Subcategories.FindAsync([id], ct);

        if (subcategory is null || subcategory.CategoryId != categoryId)
        {
            return false;
        }

        db.Subcategories.Remove(subcategory);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
