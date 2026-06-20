using System.Reflection;

namespace WiSave.Incomes.WebApi.Endpoints;

public static class EndpointModuleExtensions
{
    public static void MapWebApiEndpoints(this IEndpointRouteBuilder app)
    {
        var endpointModuleType = typeof(IEndpointModule);
        var moduleTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false }
                && endpointModuleType.IsAssignableFrom(type))
            .OrderBy(type => type.FullName, StringComparer.Ordinal);

        foreach (var moduleType in moduleTypes)
        {
            var module = (IEndpointModule)Activator.CreateInstance(moduleType)!;
            module.MapEndpoints(app);
        }
    }
}
