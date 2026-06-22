namespace WiSave.Incomes.Core.Application.Categories;

public interface ICategoryRepository
{
    Task CreateAsync(
        Guid id,
        Guid userId,
        string name,
        int sortOrder,
        CancellationToken ct = default);
}
