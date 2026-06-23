using Microsoft.EntityFrameworkCore;
using WiSave.Incomes.Projections.Postgres.Entities;

namespace WiSave.Incomes.Projections.Postgres;

public sealed class ProjectionsDbContext(DbContextOptions<ProjectionsDbContext> options) : DbContext(options)
{
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("projections");

        modelBuilder.Entity<CategoryEntity>(e =>
        {
            e.ToTable("categories");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.SortOrder).HasDefaultValue(0);
            e.HasIndex(x => x.UserId);
            e.ComplexCollection(x => x.Subcategories, subcategories =>
            {
                subcategories.ToJson("subcategories");
            });
        });
    }
}
