namespace WiSave.Incomes.Core.Infrastructure.Postgres.Entities;

public sealed class IncomeSourceEntity
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
