namespace WiSave.Incomes.Contracts.Commands;

public sealed record UpdateCategory(
    Guid Id,
    Guid UserId,
    string Name,
    int? SortOrder = null);
