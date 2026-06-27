using Microsoft.AspNetCore.Mvc;
using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.Contracts.Requests;
using WiSave.Incomes.Contracts.Responses;
using WiSave.Incomes.Core.Infrastructure.Identity;
using WiSave.Incomes.WebApi.Requests.Categories;
using Wolverine;

namespace WiSave.Incomes.WebApi.Endpoints;

public sealed class CategoryEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/incomes/categories").WithTags("Categories");

        group.MapGet("/", GetAll)
            .ProducesOk<GetCategoriesResponse>(
                "ListIncomeCategories",
                "List income categories",
                "Returns income categories and subcategories for the current user.");

        group.MapPost("/", Create)
            .ProducesAccepted(
                "CreateIncomeCategory",
                "Create an income category",
                "Accepts an income category creation command for the current user.");

        group.MapPut("/{id:guid}", Update)
            .ProducesAccepted(
                "UpdateIncomeCategory",
                "Update an income category",
                "Accepts an income category update command for the current user.");

        group.MapDelete("/{id:guid}", Delete)
            .ProducesAccepted(
                "DeleteIncomeCategory",
                "Delete an income category",
                "Accepts an income category deletion command for the current user.");

        group.MapPost("/{id:guid}/subcategories", CreateSubcategory)
            .ProducesAccepted(
                "CreateIncomeSubcategory",
                "Create an income subcategory",
                "Accepts an income subcategory creation command for the current user.");

        group.MapDelete("/{id:guid}/subcategories/{subId:guid}", DeleteSubcategory)
            .ProducesAccepted(
                "DeleteIncomeSubcategory",
                "Delete an income subcategory",
                "Accepts an income subcategory deletion command for the current user.");
    }

    private static async Task<IResult> GetAll(ICurrentUser user, IMessageBus bus, CancellationToken ct)
    {
        var response = await bus.InvokeAsync<GetCategoriesResponse>(
            new GetCategoriesRequest(user.UserId),
            DeliveryOptions.RequireResponse<GetCategoriesResponse>(), ct);

        return Results.Ok(response);
    }

    private static async Task<IResult> Create([FromBody] CreateCategoryRequest request, ICurrentUser user, IMessageBus bus)
    {
        var categoryId = Guid.CreateVersion7();
        var @command = new CreateCategory(categoryId, user.UserId, request.Name, request.SortOrder);
        await bus.SendAsync(@command);
        return Results.Accepted($"/incomes/categories/{categoryId}", value: null);
    }

    private static async Task<IResult> Update(Guid id, [FromBody] UpdateCategoryRequest request, ICurrentUser user, IMessageBus bus)
    {
        var @command = new UpdateCategory(id, user.UserId, request.Name, request.SortOrder);
        await bus.SendAsync(@command);
        return Results.Accepted();
    }

    private static async Task<IResult> Delete(Guid id, ICurrentUser user, IMessageBus bus)
    {
        var @command = new DeleteCategory(id, user.UserId);
        await bus.SendAsync(@command);
        return Results.Accepted();
    }

    private static async Task<IResult> CreateSubcategory(
        Guid id,
        [FromBody] CreateSubcategoryRequest request,
        ICurrentUser user,
        IMessageBus bus)
    {
        var subcategoryId = Guid.CreateVersion7();
        var @command = new CreateSubcategory(subcategoryId, id, user.UserId, request.Name, request.SortOrder);
        await bus.SendAsync(@command);
        return Results.Accepted($"/incomes/categories/{id}/subcategories/{subcategoryId}", value: null);
    }

    private static async Task<IResult> DeleteSubcategory(Guid id, Guid subId, ICurrentUser user, IMessageBus bus)
    {
        var @command = new DeleteSubcategory(id, subId, user.UserId);
        await bus.SendAsync(@command);
        return Results.Accepted();
    }
}
