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

    public Guid UserId
    {
        get
        {
            var value = configuration["DevelopmentApiKey:UserId"] ?? "018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234";
            return Guid.TryParse(value, out var userId)
                ? userId
                : throw new InvalidOperationException("DevelopmentApiKey:UserId must be a valid GUID.");
        }
    }

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
