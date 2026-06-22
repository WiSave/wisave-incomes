namespace WiSave.Incomes.Core.Infrastructure.Postgres.Entities;

public sealed class SubcategoryEntity
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public CategoryEntity Category { get; set; } = null!;
}
