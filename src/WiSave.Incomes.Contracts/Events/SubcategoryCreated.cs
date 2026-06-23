namespace WiSave.Incomes.Contracts.Events;

public sealed record SubcategoryCreated(
    Guid Id,
    Guid CategoryId,
    Guid UserId,
    string Name,
    int SortOrder);
