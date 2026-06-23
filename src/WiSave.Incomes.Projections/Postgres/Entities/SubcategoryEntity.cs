namespace WiSave.Incomes.Projections.Postgres.Entities;

public sealed class SubcategoryEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
