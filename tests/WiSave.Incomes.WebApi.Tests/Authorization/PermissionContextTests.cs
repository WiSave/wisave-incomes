using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using WiSave.Incomes.WebApi.Authorization;

namespace WiSave.Incomes.WebApi.Tests.Authorization;

public class PermissionContextTests
{
    [Fact]
    public void HasUserId_returns_true_when_user_id_header_is_guid()
    {
        var context = CreateContext(userId: "018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");

        Assert.True(context.HasUserId);
    }

    [Fact]
    public void HasUserId_returns_false_when_user_id_header_is_not_guid()
    {
        var context = CreateContext(userId: "development-user");

        Assert.False(context.HasUserId);
    }

    private static PermissionContext CreateContext(string? userId = null)
    {
        var httpContext = new DefaultHttpContext();
        if (userId is not null)
        {
            httpContext.Request.Headers["X-User-Id"] = userId;
        }

        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        return new PermissionContext(accessor, new DevelopmentApiKeyAuthentication(
            new ConfigurationBuilder().Build(),
            new TestWebHostEnvironment(),
            accessor));
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Production";
        public string ApplicationName { get; set; } = "WiSave.Incomes.WebApi.Tests";
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
