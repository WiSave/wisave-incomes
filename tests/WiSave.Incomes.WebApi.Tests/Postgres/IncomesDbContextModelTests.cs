using Microsoft.EntityFrameworkCore;
using WiSave.Incomes.Core.Infrastructure.Postgres;
using WiSave.Incomes.Core.Infrastructure.Postgres.Entities;

namespace WiSave.Incomes.WebApi.Tests.Postgres;

public class IncomesDbContextModelTests
{
    [Fact]
    public void Category_has_one_to_many_subcategories_relationship()
    {
        var options = new DbContextOptionsBuilder<IncomesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new IncomesDbContext(options);
        var subcategory = db.Model.FindEntityType(typeof(SubcategoryEntity));
        Assert.NotNull(subcategory);

        var foreignKey = Assert.Single(
            subcategory.GetForeignKeys(),
            fk => fk.PrincipalEntityType.ClrType == typeof(CategoryEntity));

        Assert.Equal(nameof(SubcategoryEntity.CategoryId), Assert.Single(foreignKey.Properties).Name);
        Assert.Equal(nameof(SubcategoryEntity.Category), foreignKey.DependentToPrincipal?.Name);
        Assert.Equal(nameof(CategoryEntity.Subcategories), foreignKey.PrincipalToDependent?.Name);
        Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
    }
}
