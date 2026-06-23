using WiSave.Incomes.Contracts.Models;

namespace WiSave.Incomes.Contracts.Commands;

public sealed record CreateIncomeCommand(
    Money Amount,
    DateOnly IncomeDate,
    string Name,
    string? Description,
    Guid UserId,
    Guid? CategoryId,
    Guid? SubcategoryId,
    IReadOnlyCollection<string> Tags);
