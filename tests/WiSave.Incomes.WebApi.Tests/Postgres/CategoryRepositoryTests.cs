using Microsoft.EntityFrameworkCore;
using WiSave.Incomes.Core.Infrastructure.Postgres;
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
}
