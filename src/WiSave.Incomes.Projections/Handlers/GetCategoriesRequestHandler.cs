using Microsoft.EntityFrameworkCore;
using WiSave.Incomes.Contracts.Requests;
using WiSave.Incomes.Contracts.Responses;
using WiSave.Incomes.Projections.Postgres;

namespace WiSave.Incomes.Projections.Handlers;

public sealed class GetCategoriesRequestHandler(ProjectionsDbContext db)
{
    public async Task<GetCategoriesResponse> Handle(GetCategoriesRequest request)
    {
        var categories = db.Categories
            .AsNoTracking()
            .Where(x => x.UserId == request.UserId)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Name);

        return new GetCategoriesResponse(
            categories
                .Select(x => new CategoryResponse(
                    x.Id,
                    x.Name,
                    x.SortOrder,
                    x.Subcategories
                        .OrderBy(subcategory => subcategory.SortOrder)
                        .ThenBy(subcategory => subcategory.Name)
                        .Select(subcategory => new SubcategoryResponse(
                            subcategory.Id,
                            subcategory.Name,
                            subcategory.SortOrder))
                        .ToList()))
                .ToList());
    }
    
}
