namespace WiSave.Incomes.Contracts.Commands;

public sealed record DeleteSubcategory(
    Guid CategoryId,
    Guid Id,
    Guid UserId);
