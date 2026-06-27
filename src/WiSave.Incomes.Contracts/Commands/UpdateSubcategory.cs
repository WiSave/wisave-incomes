namespace WiSave.Incomes.Contracts.Commands;

public sealed record UpdateSubcategory(
    Guid CategoryId,
    Guid Id,
    Guid UserId,
    string Name,
    int? SortOrder = null);
