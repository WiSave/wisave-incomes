using WiSave.Incomes.Contracts.Models;

namespace WiSave.Incomes.Contracts.Events;

public sealed record IncomeCreated(
    IncomeId Id,
    Money Amount,
    DateOnly IncomeDate,
    string Name,
    string? Description,
    Guid UserId,
    IReadOnlyCollection<string> Tags);
