namespace WiSave.Incomes.WebApi.Endpoints;

internal static class OpenApiEndpointExtensions
{
    public static RouteHandlerBuilder ProducesOk(
        this RouteHandlerBuilder builder,
        string name,
        string summary,
        string description)
    {
        return builder
            .WithName(name)
            .WithSummary(summary)
            .WithDescription(description)
            .Produces(StatusCodes.Status200OK);
    }

    public static RouteHandlerBuilder ProducesOk<TResponse>(
        this RouteHandlerBuilder builder,
        string name,
        string summary,
        string description)
    {
        return builder
            .WithName(name)
            .WithSummary(summary)
            .WithDescription(description)
            .Produces<TResponse>(StatusCodes.Status200OK);
    }

    public static RouteHandlerBuilder ProducesCreated(
        this RouteHandlerBuilder builder,
        string name,
        string summary,
        string description)
    {
        return builder
            .WithName(name)
            .WithSummary(summary)
            .WithDescription(description)
            .Produces(StatusCodes.Status201Created);
    }
}
