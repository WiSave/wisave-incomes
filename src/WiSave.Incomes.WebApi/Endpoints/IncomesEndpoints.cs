using Microsoft.AspNetCore.Mvc;
using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.WebApi.Requests.Incomes;

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

    private static IResult Create([FromBody] CreateIncomeCommand request) => Results.StatusCode(200);

    private static IResult GetById(Guid id) => Results.StatusCode(200);

    private static IResult Update(Guid id, [FromBody] UpdateIncomeRequest request) => Results.StatusCode(200);

    private static IResult Delete(Guid id) => Results.StatusCode(200);
}
