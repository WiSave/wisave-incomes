using Microsoft.EntityFrameworkCore;
using WiSave.Incomes.Core.Infrastructure.Postgres.Entities;

namespace WiSave.Incomes.Core.Infrastructure.Postgres;

public sealed class IncomesDbContext(DbContextOptions<IncomesDbContext> options) : DbContext(options)
{
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<SubcategoryEntity> Subcategories => Set<SubcategoryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("config");

        modelBuilder.Entity<CategoryEntity>(e =>
        {
            e.ToTable("categories");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.SortOrder).HasDefaultValue(0);
            e.HasIndex(x => x.UserId);
            e.HasMany(x => x.Subcategories)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SubcategoryEntity>(e =>
        {
            e.ToTable("subcategories");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.SortOrder).HasDefaultValue(0);
            e.HasIndex(x => x.CategoryId);
        });
    }
}
