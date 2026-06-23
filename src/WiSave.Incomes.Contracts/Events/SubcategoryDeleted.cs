namespace WiSave.Incomes.Contracts.Events;

public sealed record SubcategoryDeleted(
    Guid CategoryId,
    Guid Id,
    Guid UserId);
