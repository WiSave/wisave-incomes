namespace WiSave.Incomes.Contracts.Events;

public sealed record CategoryCreated(
    Guid Id,
    Guid UserId,
    string Name,
    int SortOrder);
