namespace WiSave.Incomes.Contracts.Commands;

public sealed record CreateCategory(
    Guid Id,
    Guid UserId,
    string Name,
    int? SortOrder = null);
