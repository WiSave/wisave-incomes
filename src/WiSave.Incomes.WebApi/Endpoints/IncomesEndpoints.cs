using Microsoft.AspNetCore.Mvc;
using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.Core.Infrastructure.Identity;
using WiSave.Incomes.WebApi.Requests.Incomes;
using Wolverine;

namespace WiSave.Incomes.WebApi.Endpoints;

public sealed class IncomesEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/incomes").WithTags("Incomes");

        group.MapGet("/", GetAll)
            .ProducesOk(
                "ListIncomes",
                "List incomes",
                "Returns incomes for the current user.");

        group.MapPost("/", Create)
            .ProducesAccepted(
                "CreateIncome",
                "Create an income",
                "Accepts an income creation command for the current user.");

        group.MapGet("/{id:guid}", GetById)
            .ProducesOk(
                "GetIncome",
                "Get an income",
                "Returns a single income for the current user.");

        group.MapPut("/{id:guid}", Update)
            .ProducesOk(
                "UpdateIncome",
                "Update an income",
                "Updates an income for the current user.");

        group.MapDelete("/{id:guid}", Delete)
            .ProducesOk(
                "DeleteIncome",
                "Delete an income",
                "Deletes an income for the current user.");
    }

    private static IResult GetAll() => Results.StatusCode(200);

    private static async Task<IResult> Create([FromBody] CreateIncomeRequest request, ICurrentUser user, IMessageBus bus)
    {
        var @command = new CreateIncomeCommand(
            request.Amount,
            request.IncomeDate,
            request.Name,
            request.Description,
            user.UserId,
            request.CategoryId,
            request.SubcategoryId,
            request.Tags);

        await bus.SendAsync(@command);
        return Results.Accepted();
    }

    private static IResult GetById(Guid id) => Results.StatusCode(200);

    private static IResult Update(Guid id, [FromBody] UpdateIncomeRequest request) => Results.StatusCode(200);

    private static IResult Delete(Guid id) => Results.StatusCode(200);
}
