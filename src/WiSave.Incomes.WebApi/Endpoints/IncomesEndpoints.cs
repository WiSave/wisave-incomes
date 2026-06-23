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

        group.MapGet("/", GetAll);
        group.MapPost("/", Create);
        group.MapGet("/{id:guid}", GetById);
        group.MapPut("/{id:guid}", Update);
        group.MapDelete("/{id:guid}", Delete);
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
        return Results.StatusCode(200);
    }

    private static IResult GetById(Guid id) => Results.StatusCode(200);

    private static IResult Update(Guid id, [FromBody] UpdateIncomeRequest request) => Results.StatusCode(200);

    private static IResult Delete(Guid id) => Results.StatusCode(200);
}
