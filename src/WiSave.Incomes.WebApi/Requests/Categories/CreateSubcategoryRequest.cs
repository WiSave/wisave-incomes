namespace WiSave.Incomes.WebApi.Requests.Categories;

public sealed record CreateSubcategoryRequest(string Name, int? SortOrder = null);
