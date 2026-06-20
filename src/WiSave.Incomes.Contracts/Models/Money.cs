namespace WiSave.Incomes.Contracts.Models;

public sealed record Money(decimal Amount, Currency Currency)
{
    public decimal Amount { get; init; } = Amount >= 0
        ? Amount
        : throw new ArgumentOutOfRangeException(nameof(Amount), Amount, "Amount cannot be negative.");
}
