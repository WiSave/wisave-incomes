using Microsoft.EntityFrameworkCore;

namespace WiSave.Incomes.Projections;

public sealed class ProjectionsDbContext(DbContextOptions<ProjectionsDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("projections");
    }
}
