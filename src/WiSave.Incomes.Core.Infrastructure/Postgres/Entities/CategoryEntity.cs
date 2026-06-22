namespace WiSave.Incomes.Core.Infrastructure.Postgres.Entities;

public sealed class CategoryEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public ICollection<SubcategoryEntity> Subcategories { get; set; } = new List<SubcategoryEntity>();
}
