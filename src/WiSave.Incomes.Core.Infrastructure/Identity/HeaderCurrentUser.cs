using Microsoft.AspNetCore.Http;

namespace WiSave.Incomes.Core.Infrastructure.Identity;

public sealed class HeaderCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException("X-User-Id header is missing.");
            }

            return Guid.TryParse(value, out var userId)
                ? userId
                : throw new InvalidOperationException("X-User-Id header must be a valid GUID.");
        }
    }

    public string Email => httpContextAccessor.HttpContext?.Request.Headers["X-User-Email"].FirstOrDefault()
        ?? string.Empty;
}
