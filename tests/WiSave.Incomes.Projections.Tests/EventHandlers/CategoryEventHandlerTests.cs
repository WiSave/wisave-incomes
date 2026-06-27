using Microsoft.EntityFrameworkCore;
using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Projections.EventHandlers;
using WiSave.Incomes.Projections.Postgres;
using WiSave.Incomes.Projections.Postgres.Entities;

namespace WiSave.Incomes.Projections.Tests.EventHandlers;

public class CategoryEventHandlerTests
{
    private static readonly Guid UserId = Guid.Parse("118f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
    private static readonly Guid OtherUserId = Guid.Parse("128f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
    private static readonly Guid CategoryId = Guid.Parse("218f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
    private static readonly Guid SubcategoryId = Guid.Parse("318f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");

    [Fact]
    public async Task CategoryUpdated_updates_category_projection()
    {
        await using var db = CreateDbContext();
        db.Categories.Add(new CategoryEntity
        {
            Id = CategoryId,
            UserId = UserId,
            Name = "Old",
            SortOrder = 1,
            Subcategories = []
        });
        await db.SaveChangesAsync();

        var handler = new CategoryUpdatedEventHandler(db);

        await handler.Handle(new CategoryUpdated(CategoryId, UserId, "Salary", 4), CancellationToken.None);

        var category = Assert.Single(await db.Categories.ToListAsync());
        Assert.Equal("Salary", category.Name);
        Assert.Equal(4, category.SortOrder);
    }

    [Fact]
    public async Task CategoryUpdated_throws_when_category_projection_is_missing()
    {
        await using var db = CreateDbContext();
        var handler = new CategoryUpdatedEventHandler(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new CategoryUpdated(CategoryId, UserId, "Salary", 4), CancellationToken.None));
    }

    [Fact]
    public async Task CategoryUpdated_throws_when_category_projection_belongs_to_another_user()
    {
        await using var db = CreateDbContext();
        db.Categories.Add(new CategoryEntity
        {
            Id = CategoryId,
            UserId = OtherUserId,
            Name = "Old",
            SortOrder = 1,
            Subcategories = []
        });
        await db.SaveChangesAsync();
        var handler = new CategoryUpdatedEventHandler(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new CategoryUpdated(CategoryId, UserId, "Salary", 4), CancellationToken.None));
    }

    [Fact]
    public async Task CategoryDeleted_removes_category_projection()
    {
        await using var db = CreateDbContext();
        db.Categories.Add(new CategoryEntity
        {
            Id = CategoryId,
            UserId = UserId,
            Name = "Salary",
            SortOrder = 1,
            Subcategories = []
        });
        await db.SaveChangesAsync();

        var handler = new CategoryDeletedEventHandler(db);

        await handler.Handle(new CategoryDeleted(CategoryId, UserId), CancellationToken.None);

        Assert.Empty(await db.Categories.ToListAsync());
    }

    [Fact]
    public async Task CategoryDeleted_throws_when_category_projection_is_missing()
    {
        await using var db = CreateDbContext();
        var handler = new CategoryDeletedEventHandler(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new CategoryDeleted(CategoryId, UserId), CancellationToken.None));
    }

    [Fact]
    public async Task CategoryDeleted_throws_when_category_projection_belongs_to_another_user()
    {
        await using var db = CreateDbContext();
        db.Categories.Add(new CategoryEntity
        {
            Id = CategoryId,
            UserId = OtherUserId,
            Name = "Salary",
            SortOrder = 1,
            Subcategories = []
        });
        await db.SaveChangesAsync();
        var handler = new CategoryDeletedEventHandler(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new CategoryDeleted(CategoryId, UserId), CancellationToken.None));
    }

    [Fact]
    public async Task SubcategoryCreated_adds_subcategory_projection()
    {
        await using var db = CreateDbContext();
        db.Categories.Add(new CategoryEntity
        {
            Id = CategoryId,
            UserId = UserId,
            Name = "Salary",
            SortOrder = 1,
            Subcategories = []
        });
        await db.SaveChangesAsync();

        var handler = new SubcategoryCreatedEventHandler(db);

        await handler.Handle(
            new SubcategoryCreated(SubcategoryId, CategoryId, UserId, "Base pay", 2),
            CancellationToken.None);

        var category = Assert.Single(await db.Categories.ToListAsync());
        var subcategory = Assert.Single(category.Subcategories);
        Assert.Equal(SubcategoryId, subcategory.Id);
        Assert.Equal("Base pay", subcategory.Name);
        Assert.Equal(2, subcategory.SortOrder);
    }

    [Fact]
    public async Task SubcategoryCreated_throws_when_parent_category_projection_is_missing()
    {
        await using var db = CreateDbContext();
        var handler = new SubcategoryCreatedEventHandler(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(
                new SubcategoryCreated(SubcategoryId, CategoryId, UserId, "Base pay", 2),
                CancellationToken.None));
    }

    [Fact]
    public async Task SubcategoryCreated_throws_when_parent_category_projection_belongs_to_another_user()
    {
        await using var db = CreateDbContext();
        db.Categories.Add(new CategoryEntity
        {
            Id = CategoryId,
            UserId = OtherUserId,
            Name = "Salary",
            SortOrder = 1,
            Subcategories = []
        });
        await db.SaveChangesAsync();
        var handler = new SubcategoryCreatedEventHandler(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(
                new SubcategoryCreated(SubcategoryId, CategoryId, UserId, "Base pay", 2),
                CancellationToken.None));
    }

    [Fact]
    public async Task SubcategoryDeleted_removes_subcategory_projection()
    {
        await using var db = CreateDbContext();
        db.Categories.Add(new CategoryEntity
        {
            Id = CategoryId,
            UserId = UserId,
            Name = "Salary",
            SortOrder = 1,
            Subcategories =
            [
                new SubcategoryEntity
                {
                    Id = SubcategoryId,
                    Name = "Base pay",
                    SortOrder = 2
                }
            ]
        });
        await db.SaveChangesAsync();

        var handler = new SubcategoryDeletedEventHandler(db);

        await handler.Handle(new SubcategoryDeleted(CategoryId, SubcategoryId, UserId), CancellationToken.None);

        var category = Assert.Single(await db.Categories.ToListAsync());
        Assert.Empty(category.Subcategories);
    }

    [Fact]
    public async Task SubcategoryDeleted_throws_when_parent_category_projection_is_missing()
    {
        await using var db = CreateDbContext();
        var handler = new SubcategoryDeletedEventHandler(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new SubcategoryDeleted(CategoryId, SubcategoryId, UserId), CancellationToken.None));
    }

    [Fact]
    public async Task SubcategoryDeleted_throws_when_parent_category_projection_belongs_to_another_user()
    {
        await using var db = CreateDbContext();
        db.Categories.Add(new CategoryEntity
        {
            Id = CategoryId,
            UserId = OtherUserId,
            Name = "Salary",
            SortOrder = 1,
            Subcategories =
            [
                new SubcategoryEntity
                {
                    Id = SubcategoryId,
                    Name = "Base pay",
                    SortOrder = 2
                }
            ]
        });
        await db.SaveChangesAsync();
        var handler = new SubcategoryDeletedEventHandler(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new SubcategoryDeleted(CategoryId, SubcategoryId, UserId), CancellationToken.None));
    }

    [Fact]
    public async Task SubcategoryDeleted_throws_when_subcategory_projection_is_missing()
    {
        await using var db = CreateDbContext();
        db.Categories.Add(new CategoryEntity
        {
            Id = CategoryId,
            UserId = UserId,
            Name = "Salary",
            SortOrder = 1,
            Subcategories = []
        });
        await db.SaveChangesAsync();
        var handler = new SubcategoryDeletedEventHandler(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new SubcategoryDeleted(CategoryId, SubcategoryId, UserId), CancellationToken.None));
    }

    private static ProjectionsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ProjectionsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ProjectionsDbContext(options);
    }
}
