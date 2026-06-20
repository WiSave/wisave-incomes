using Microsoft.EntityFrameworkCore;
using WiSave.Incomes.Core.Infrastructure.Postgres.Entities;

namespace WiSave.Incomes.Core.Infrastructure.Postgres;

public sealed class IncomesDbContext(DbContextOptions<IncomesDbContext> options) : DbContext(options)
{
    public DbSet<IncomeSourceEntity> IncomeSources => Set<IncomeSourceEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("config");

        modelBuilder.Entity<IncomeSourceEntity>(e =>
        {
            e.ToTable("income_sources");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasMaxLength(64);
            e.Property(x => x.UserId).HasMaxLength(64).IsRequired();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.SortOrder).HasDefaultValue(0);
            e.HasIndex(x => x.UserId);
        });
    }
}
