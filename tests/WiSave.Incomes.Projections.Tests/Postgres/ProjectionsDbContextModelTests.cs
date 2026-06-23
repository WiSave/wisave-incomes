using Microsoft.EntityFrameworkCore;
using WiSave.Incomes.Projections.Postgres;

namespace WiSave.Incomes.Projections.Tests.Postgres;

public class ProjectionsDbContextModelTests
{
    [Fact]
    public void Category_projection_stores_subcategories_as_jsonb()
    {
        var options = new DbContextOptionsBuilder<ProjectionsDbContext>()
            .UseNpgsql("Host=localhost;Database=wisave_incomes_projections_test")
            .Options;

        using var db = new ProjectionsDbContext(options);

        var createScript = db.Database.GenerateCreateScript();

        Assert.Contains("categories", createScript);
        Assert.Contains("subcategories jsonb", createScript);
        Assert.DoesNotContain("CREATE TABLE projections.subcategories", createScript);
        Assert.DoesNotContain("CREATE TABLE projections.\"subcategories\"", createScript);
    }
}
