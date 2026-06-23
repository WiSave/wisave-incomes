namespace WiSave.Incomes.Contracts.Responses;

public sealed record GetCategoriesResponse(IReadOnlyList<CategoryResponse> Categories);

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    int SortOrder,
    IReadOnlyList<SubcategoryResponse> Subcategories);

public sealed record SubcategoryResponse(
    Guid Id,
    string Name,
    int SortOrder);
