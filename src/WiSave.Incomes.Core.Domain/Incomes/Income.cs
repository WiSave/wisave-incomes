using WiSave.Framework.Domain.Core;
using WiSave.Incomes.Contracts.Events;
using WiSave.Incomes.Contracts.Models;

namespace WiSave.Incomes.Core.Domain.Incomes;

public sealed class Income : AggregateRoot<IncomeId>
{
    public Money Amount { get; private set; } = default!;
    public DateOnly IncomeDate { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid UserId { get; private set; }
    public IReadOnlyCollection<string> Tags { get; private set; } = [];
    public Guid? CategoryId { get; private set; }
    public Guid? SubcategoryId { get; private set; }

    public static Income Create(
        IncomeId id,
        Money amount,
        DateOnly incomeDate,
        string name,
        string? description,
        Guid userId,
        IReadOnlyCollection<string> tags,
        Guid? categoryId,
        Guid? subcategoryId)
    {
        var income = new Income();
        income.RaiseEvent(new IncomeCreated(
            id,
            amount,
            incomeDate,
            name,
            description,
            userId,
            tags.ToArray(),
            categoryId,
            subcategoryId));

        return income;
    }

    public void Apply(IncomeCreated e)
    {
        Id = e.Id;
        Amount = e.Amount;
        IncomeDate = e.IncomeDate;
        Name = e.Name;
        Description = e.Description;
        UserId = e.UserId;
        Tags = e.Tags.ToArray();
        CategoryId = e.CategoryId;
        SubcategoryId = e.SubcategoryId;
    }
}
