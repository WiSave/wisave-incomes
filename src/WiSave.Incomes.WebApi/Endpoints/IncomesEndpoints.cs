using Microsoft.AspNetCore.Mvc;
using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.WebApi.Authorization;
using Wolverine;

namespace WiSave.Incomes.WebApi.Endpoints;

public sealed class IncomesEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/incomes").WithTags("Incomes");

        group.MapPost("/", Create).RequirePermission(Permissions.Incomes.Write);
    }

    private static async Task<IResult> Create([FromBody] CreateIncomeCommand command, [FromServices] IMessageBus bus)
    {
        await bus.PublishAsync(command);
        return Results.Accepted();
    }
}
