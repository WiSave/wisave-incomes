namespace WiSave.Incomes.Core.Application.Categories;

public interface ICategoryRepository
{
    Task CreateAsync(
        Guid id,
        Guid userId,
        string name,
        int sortOrder,
        CancellationToken ct = default);

    Task<bool> UpdateAsync(
        Guid id,
        Guid userId,
        string name,
        int sortOrder,
        CancellationToken ct = default);

    Task<bool> DeleteAsync(
        Guid id,
        Guid userId,
        CancellationToken ct = default);

    Task<bool> CreateSubcategoryAsync(
        Guid id,
        Guid categoryId,
        Guid userId,
        string name,
        int sortOrder,
        CancellationToken ct = default);

    Task<bool> DeleteSubcategoryAsync(
        Guid categoryId,
        Guid id,
        Guid userId,
        CancellationToken ct = default);
}
