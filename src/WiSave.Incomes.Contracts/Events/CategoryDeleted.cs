namespace WiSave.Incomes.Contracts.Events;

public sealed record CategoryDeleted(
    Guid Id,
    Guid UserId);
