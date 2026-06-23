using WiSave.Incomes.Contracts.Models;

namespace WiSave.Incomes.WebApi.Requests.Incomes;

public sealed record CreateIncomeRequest(
    Money Amount,
    DateOnly IncomeDate,
    string Name,
    string? Description,
    Guid? CategoryId,
    Guid? SubcategoryId,
    IReadOnlyCollection<string> Tags);
