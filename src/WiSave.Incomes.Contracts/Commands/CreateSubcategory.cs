namespace WiSave.Incomes.Contracts.Commands;

public sealed record CreateSubcategory(
    Guid Id,
    Guid CategoryId,
    Guid UserId,
    string Name,
    int? SortOrder = null);
