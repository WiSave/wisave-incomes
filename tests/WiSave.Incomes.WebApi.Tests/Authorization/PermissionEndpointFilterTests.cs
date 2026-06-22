using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WiSave.Incomes.WebApi.Authorization;

namespace WiSave.Incomes.WebApi.Tests.Authorization;

public class PermissionEndpointFilterTests : IAsyncLifetime
{
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<DevelopmentApiKeyAuthentication>();
        builder.Services.AddScoped<PermissionContext>();

        _app = builder.Build();
        _app.MapGet("/test", () => Results.Ok("allowed"))
            .RequirePermission(Permissions.Incomes.Read);

        await _app.StartAsync();

        var address = _app.Services.GetRequiredService<IServer>()
            .Features.Get<IServerAddressesFeature>()!.Addresses.Single();
        _client = new HttpClient { BaseAddress = new Uri(address) };
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
    }

    [Fact]
    public async Task Request_with_required_permission_returns_ok()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/test");
        request.Headers.Add("X-User-Id", "018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
        request.Headers.Add("X-User-Permissions", Permissions.Incomes.Read);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Request_without_required_permission_returns_forbidden()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/test");
        request.Headers.Add("X-User-Id", "018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
        request.Headers.Add("X-User-Permissions", "other:read");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
