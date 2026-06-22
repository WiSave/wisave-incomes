using WiSave.Incomes.Core.Infrastructure.Identity;

namespace WiSave.Incomes.WebApi.Authorization;

public sealed class HeaderOrDevelopmentCurrentUser(
    IHttpContextAccessor httpContextAccessor,
    DevelopmentApiKeyAuthentication developmentApiKey) : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return Guid.TryParse(value, out var userId)
                    ? userId
                    : throw new InvalidOperationException("X-User-Id header must be a valid GUID.");
            }

            return developmentApiKey.IsAuthorized
                ? developmentApiKey.UserId
                : throw new InvalidOperationException("X-User-Id header is missing.");
        }
    }

    public string Email => httpContextAccessor.HttpContext?.Request.Headers["X-User-Email"].FirstOrDefault()
        ?? (developmentApiKey.IsAuthorized ? developmentApiKey.Email : string.Empty);
}
