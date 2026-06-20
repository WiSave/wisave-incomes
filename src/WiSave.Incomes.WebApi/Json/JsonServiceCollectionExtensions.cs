using System.Text.Json.Serialization;

namespace WiSave.Incomes.WebApi.Json;

public static class JsonServiceCollectionExtensions
{
    public static IServiceCollection AddJson(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        return services;
    }
}
