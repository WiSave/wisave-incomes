namespace WiSave.Incomes.Contracts.Models;

public sealed record UserId(string Value)
{
    public static explicit operator string(UserId id) => id.Value;
    public static explicit operator UserId(string value) => new(value);
}

public sealed record IncomeId(Guid Value)
{
    public static explicit operator Guid(IncomeId id) => id.Value;
    public static explicit operator IncomeId(Guid value) => new(value);
}
