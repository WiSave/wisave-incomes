using WiSave.Incomes.Contracts.Models;

namespace WiSave.Incomes.WebApi.Requests.Incomes;

public sealed record UpdateIncomeRequest(
    Money Amount,
    DateOnly IncomeDate,
    string Name,
    string? Description,
    IReadOnlyCollection<string> Tags);
