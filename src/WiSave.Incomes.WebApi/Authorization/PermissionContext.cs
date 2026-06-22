namespace WiSave.Incomes.WebApi.Authorization;

public sealed class PermissionContext(
    IHttpContextAccessor httpContextAccessor,
    DevelopmentApiKeyAuthentication developmentApiKey)
{
    private HashSet<string>? _permissions;

    public IReadOnlySet<string> Permissions => _permissions ??= ParsePermissions();

    public bool HasUserId => developmentApiKey.IsAuthorized || HasValidUserIdHeader();

    public bool HasPermission(string permission) =>
        developmentApiKey.IsAuthorized || Permissions.Contains("*") || Permissions.Contains(permission);

    private HashSet<string> ParsePermissions()
    {
        var header = httpContextAccessor.HttpContext?.Request.Headers["X-User-Permissions"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(header))
        {
            return [];
        }

        return header.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private bool HasValidUserIdHeader()
    {
        var value = httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].FirstOrDefault();
        return Guid.TryParse(value, out _);
    }
}
