namespace WiSave.Incomes.Contracts.Commands;

public sealed record DeleteCategory(
    Guid Id,
    Guid UserId);
