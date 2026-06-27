using Microsoft.EntityFrameworkCore;
using WiSave.Incomes.Core.Infrastructure.Postgres;
using WiSave.Incomes.Core.Infrastructure.Postgres.Entities;
using WiSave.Incomes.Core.Infrastructure.Postgres.Repositories;

namespace WiSave.Incomes.WebApi.Tests.Postgres;

public class CategoryRepositoryTests
{
    [Fact]
    public async Task CreateAsync_saves_category()
    {
        var options = new DbContextOptionsBuilder<IncomesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new IncomesDbContext(options);
        var repository = new CategoryRepository(db);
        var categoryId = Guid.Parse("018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
        var userId = Guid.Parse("118f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");

        await repository.CreateAsync(categoryId, userId, "Salary", 3, CancellationToken.None);

        var category = Assert.Single(await db.Categories.ToListAsync());
        Assert.Equal(categoryId, category.Id);
        Assert.Equal(userId, category.UserId);
        Assert.Equal("Salary", category.Name);
        Assert.Equal(3, category.SortOrder);
    }

    [Fact]
    public async Task UpdateSubcategoryAsync_updates_subcategory_for_owning_user()
    {
        var options = new DbContextOptionsBuilder<IncomesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new IncomesDbContext(options);
        var repository = new CategoryRepository(db);
        var categoryId = Guid.Parse("018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
        var subcategoryId = Guid.Parse("218f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
        var userId = Guid.Parse("118f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
        db.Categories.Add(new CategoryEntity
        {
            Id = categoryId,
            UserId = userId,
            Name = "Salary",
            SortOrder = 1,
            Subcategories =
            [
                new SubcategoryEntity
                {
                    Id = subcategoryId,
                    Name = "Base pay",
                    SortOrder = 2
                }
            ]
        });
        await db.SaveChangesAsync();

        var updated = await repository.UpdateSubcategoryAsync(categoryId, subcategoryId, userId, "Bonus", 5, CancellationToken.None);

        Assert.True(updated);
        var subcategory = Assert.Single(await db.Subcategories.ToListAsync());
        Assert.Equal(subcategoryId, subcategory.Id);
        Assert.Equal(categoryId, subcategory.CategoryId);
        Assert.Equal("Bonus", subcategory.Name);
        Assert.Equal(5, subcategory.SortOrder);
    }
}
