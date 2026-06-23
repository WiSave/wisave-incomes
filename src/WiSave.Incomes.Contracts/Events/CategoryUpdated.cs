namespace WiSave.Incomes.Contracts.Events;

public sealed record CategoryUpdated(
    Guid Id,
    Guid UserId,
    string Name,
    int SortOrder);
