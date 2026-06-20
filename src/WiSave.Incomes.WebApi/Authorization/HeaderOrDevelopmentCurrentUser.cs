using WiSave.Incomes.Core.Infrastructure.Identity;

namespace WiSave.Incomes.WebApi.Authorization;

public sealed class HeaderOrDevelopmentCurrentUser(
    IHttpContextAccessor httpContextAccessor,
    DevelopmentApiKeyAuthentication developmentApiKey) : ICurrentUser
{
    public string UserId => httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].FirstOrDefault()
        ?? (developmentApiKey.IsAuthorized ? developmentApiKey.UserId : throw new InvalidOperationException("X-User-Id header is missing."));

    public string Email => httpContextAccessor.HttpContext?.Request.Headers["X-User-Email"].FirstOrDefault()
        ?? (developmentApiKey.IsAuthorized ? developmentApiKey.Email : string.Empty);
}
