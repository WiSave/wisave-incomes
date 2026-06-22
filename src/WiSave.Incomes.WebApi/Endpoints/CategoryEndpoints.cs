using Microsoft.AspNetCore.Mvc;
using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.Core.Infrastructure.Identity;
using WiSave.Incomes.WebApi.Requests.Categories;
using Wolverine;

namespace WiSave.Incomes.WebApi.Endpoints;

public sealed class CategoryEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/incomes/categories").WithTags("Categories");

        group.MapGet("/", GetAll);
        group.MapPost("/", Create);
        group.MapPut("/{id:guid}", Update);
        group.MapDelete("/{id:guid}", Delete);
        group.MapPost("/{id:guid}/subcategories", CreateSubcategory);
        group.MapDelete("/{id:guid}/subcategories/{subId:guid}", DeleteSubcategory);
    }

    private static IResult GetAll() => Results.StatusCode(200);

    private static async Task<IResult> Create(
        [FromBody] CreateCategoryRequest request,
        ICurrentUser user,
        IMessageBus bus)
    {
        var categoryId = Guid.CreateVersion7();
        var @command = new CreateCategory(categoryId, user.UserId, request.Name, request.SortOrder);
        await bus.SendAsync(@command);
        return Results.Created($"/incomes/categories/{categoryId}", value: null);
    }

    private static IResult Update(Guid id, [FromBody] UpdateCategoryRequest request) => Results.StatusCode(200);

    private static IResult Delete(Guid id) => Results.StatusCode(200);

    private static IResult CreateSubcategory(Guid id, [FromBody] CreateSubcategoryRequest request) => Results.StatusCode(200);

    private static IResult DeleteSubcategory(Guid id, Guid subId) => Results.StatusCode(200);
}
