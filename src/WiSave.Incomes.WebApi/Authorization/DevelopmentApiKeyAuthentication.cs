using Microsoft.Extensions.Primitives;

namespace WiSave.Incomes.WebApi.Authorization;

public sealed class DevelopmentApiKeyAuthentication(
    IConfiguration configuration,
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor)
{
    public const string HeaderName = "X-Dev-Api-Key";

    public bool IsAuthorized => environment.IsDevelopment()
        && !string.IsNullOrWhiteSpace(ApiKey)
        && TryGetHeaderValue(out var value)
        && StringComparer.Ordinal.Equals(value, ApiKey);

    public string UserId => configuration["DevelopmentApiKey:UserId"] ?? "development-user";

    public string Email => configuration["DevelopmentApiKey:Email"] ?? "development@wisave.local";

    private string? ApiKey => configuration["DevelopmentApiKey:Key"];

    private bool TryGetHeaderValue(out string? value)
    {
        value = null;

        if (httpContextAccessor.HttpContext?.Request.Headers.TryGetValue(HeaderName, out StringValues values) != true)
        {
            return false;
        }

        value = values.FirstOrDefault();
        return !string.IsNullOrWhiteSpace(value);
    }
}
