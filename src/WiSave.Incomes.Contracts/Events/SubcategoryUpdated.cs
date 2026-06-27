namespace WiSave.Incomes.Contracts.Events;

public sealed record SubcategoryUpdated(
    Guid CategoryId,
    Guid Id,
    Guid UserId,
    string Name,
    int SortOrder);
