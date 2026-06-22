namespace WiSave.Incomes.Contracts.Models;

public sealed record UserId(Guid Value)
{
    public static explicit operator Guid(UserId id) => id.Value;
    public static explicit operator UserId(Guid value) => new(value);
}

public sealed record IncomeId(Guid Value)
{
    public static explicit operator Guid(IncomeId id) => id.Value;
    public static explicit operator IncomeId(Guid value) => new(value);
}
