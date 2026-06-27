using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using WiSave.Incomes.Contracts.Commands;
using WiSave.Incomes.Contracts.Requests;
using WiSave.Incomes.Contracts.Responses;
using WiSave.Incomes.Core.Infrastructure.Identity;
using WiSave.Incomes.WebApi.Endpoints;
using WiSave.Incomes.WebApi.Requests.Categories;
using WiSave.Incomes.WebApi.Requests.Incomes;
using Wolverine;

namespace WiSave.Incomes.WebApi.Tests.Endpoints;

public class CategoryEndpointsTests : IAsyncLifetime
{
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddSingleton<CapturingMessageBus>();
        builder.Services.AddSingleton<IMessageBus>(sp => sp.GetRequiredService<CapturingMessageBus>());
        builder.Services.AddSingleton<ICurrentUser>(new TestCurrentUser(
            Guid.Parse("118f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234")));
        _app = builder.Build();
        new IncomesEndpoints().MapEndpoints(_app);
        new CategoryEndpoints().MapEndpoints(_app);

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
    public async Task Endpoints_return_empty_ok_without_domain_logic()
    {
        var cases = new[]
        {
            new EndpointCase(HttpMethod.Post, "/incomes", """{"amount":{"amount":100,"currency":0},"incomeDate":"2026-06-21","name":"Salary","description":null,"userId":"018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234","tags":[]}"""),
            new EndpointCase(HttpMethod.Get, "/incomes"),
            new EndpointCase(HttpMethod.Get, "/incomes/018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234"),
            new EndpointCase(HttpMethod.Put, "/incomes/018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234", """{"amount":{"amount":100,"currency":0},"incomeDate":"2026-06-21","name":"Salary","description":null,"tags":[]}"""),
            new EndpointCase(HttpMethod.Delete, "/incomes/018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234"),
            new EndpointCase(
                HttpMethod.Post,
                "/incomes/categories",
                """{"name":"Salary"}""",
                HttpStatusCode.Created),
            new EndpointCase(HttpMethod.Put, "/incomes/categories/018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234", """{"name":"Salary"}"""),
            new EndpointCase(HttpMethod.Delete, "/incomes/categories/018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234"),
            new EndpointCase(
                HttpMethod.Post,
                "/incomes/categories/018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234/subcategories",
                """{"name":"Base pay"}""",
                HttpStatusCode.Created),
            new EndpointCase(HttpMethod.Delete, "/incomes/categories/018f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234/subcategories/118f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234"),
        };

        foreach (var endpointCase in cases)
        {
            using var request = CreateRequest(endpointCase);
            using var response = await _client.SendAsync(request);

            Assert.Equal(endpointCase.ExpectedStatus, response.StatusCode);
            Assert.Empty(await response.Content.ReadAsStringAsync());
        }
    }

    [Fact]
    public async Task CreateIncome_sends_create_income_command_with_category_references()
    {
        var categoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var subcategoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        using var request = CreateRequest(new EndpointCase(
            HttpMethod.Post,
            "/incomes",
            $$"""
              {
                "amount":{"amount":12000,"currency":0},
                "incomeDate":"2026-06-23",
                "name":"Salary",
                "description":null,
                "categoryId":"{{categoryId}}",
                "subcategoryId":"{{subcategoryId}}",
                "tags":["Job","Monthly salary"]
              }
              """));

        using var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var bus = _app.Services.GetRequiredService<CapturingMessageBus>();
        var command = Assert.IsType<CreateIncomeCommand>(bus.Sent);
        Assert.Equal(categoryId, command.CategoryId);
        Assert.Equal(subcategoryId, command.SubcategoryId);
        Assert.Equal(Guid.Parse("118f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234"), command.UserId);
        Assert.Equal(["Job", "Monthly salary"], command.Tags);
    }

    [Fact]
    public async Task GetCategories_sends_request_and_returns_projection_response()
    {
        var categoryId = Guid.Parse("218f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
        var subcategoryId = Guid.Parse("318f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
        var bus = _app.Services.GetRequiredService<CapturingMessageBus>();
        bus.CategoriesResponse = new GetCategoriesResponse([
            new CategoryResponse(
                categoryId,
                "Salary",
                1,
                [
                    new SubcategoryResponse(subcategoryId, "Base pay", 2)
                ])
        ]);

        using var response = await _client.GetAsync("/incomes/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var request = Assert.IsType<GetCategoriesRequest>(bus.Invoked);
        Assert.Equal(Guid.Parse("118f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234"), request.UserId);
        Assert.Equal(typeof(GetCategoriesResponse), bus.InvokeOptions?.ResponseType);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("Salary", json);
        Assert.Contains("Base pay", json);
    }

    [Fact]
    public async Task CreateCategory_sends_create_category_command()
    {
        using var request = CreateRequest(new EndpointCase(
            HttpMethod.Post,
            "/incomes/categories",
            """{"name":"Salary","sortOrder":3}"""));

        using var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var bus = _app.Services.GetRequiredService<CapturingMessageBus>();
        Assert.Null(bus.Published);
        var command = Assert.IsType<CreateCategory>(bus.Sent);
        Assert.NotEqual(Guid.Empty, command.Id);
        Assert.Equal(Guid.Parse("118f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234"), command.UserId);
        Assert.Equal("Salary", command.Name);
        Assert.Equal(3, command.SortOrder);
        Assert.EndsWith($"/incomes/categories/{command.Id}", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task UpdateCategory_sends_update_category_command()
    {
        var categoryId = Guid.Parse("218f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
        using var request = CreateRequest(new EndpointCase(
            HttpMethod.Put,
            $"/incomes/categories/{categoryId}",
            """{"name":"Salary","sortOrder":4}"""));

        using var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync());
        var bus = _app.Services.GetRequiredService<CapturingMessageBus>();
        var command = Assert.IsType<UpdateCategory>(bus.Sent);
        Assert.Equal(categoryId, command.Id);
        Assert.Equal(Guid.Parse("118f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234"), command.UserId);
        Assert.Equal("Salary", command.Name);
        Assert.Equal(4, command.SortOrder);
    }

    [Fact]
    public async Task DeleteCategory_sends_delete_category_command()
    {
        var categoryId = Guid.Parse("218f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");

        using var response = await _client.DeleteAsync($"/incomes/categories/{categoryId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync());
        var bus = _app.Services.GetRequiredService<CapturingMessageBus>();
        var command = Assert.IsType<DeleteCategory>(bus.Sent);
        Assert.Equal(categoryId, command.Id);
        Assert.Equal(Guid.Parse("118f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234"), command.UserId);
    }

    [Fact]
    public async Task CreateSubcategory_sends_create_subcategory_command()
    {
        var categoryId = Guid.Parse("218f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
        using var request = CreateRequest(new EndpointCase(
            HttpMethod.Post,
            $"/incomes/categories/{categoryId}/subcategories",
            """{"name":"Base pay","sortOrder":2}"""));

        using var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var bus = _app.Services.GetRequiredService<CapturingMessageBus>();
        var command = Assert.IsType<CreateSubcategory>(bus.Sent);
        Assert.NotEqual(Guid.Empty, command.Id);
        Assert.Equal(categoryId, command.CategoryId);
        Assert.Equal(Guid.Parse("118f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234"), command.UserId);
        Assert.Equal("Base pay", command.Name);
        Assert.Equal(2, command.SortOrder);
        Assert.EndsWith($"/incomes/categories/{categoryId}/subcategories/{command.Id}", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task DeleteSubcategory_sends_delete_subcategory_command()
    {
        var categoryId = Guid.Parse("218f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");
        var subcategoryId = Guid.Parse("318f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234");

        using var response = await _client.DeleteAsync($"/incomes/categories/{categoryId}/subcategories/{subcategoryId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync());
        var bus = _app.Services.GetRequiredService<CapturingMessageBus>();
        var command = Assert.IsType<DeleteSubcategory>(bus.Sent);
        Assert.Equal(categoryId, command.CategoryId);
        Assert.Equal(subcategoryId, command.Id);
        Assert.Equal(Guid.Parse("118f7e8d-7b41-7c3a-9f0d-0b5e6a8c1234"), command.UserId);
    }

    [Theory]
    [InlineData("POST", "/incomes", typeof(CreateIncomeRequest))]
    [InlineData("PUT", "/incomes/{id:guid}", typeof(UpdateIncomeRequest))]
    [InlineData("POST", "/incomes/categories", typeof(CreateCategoryRequest))]
    [InlineData("PUT", "/incomes/categories/{id:guid}", typeof(UpdateCategoryRequest))]
    [InlineData("POST", "/incomes/categories/{id:guid}/subcategories", typeof(CreateSubcategoryRequest))]
    public void Mutating_endpoints_keep_request_body_contracts(
        string method,
        string routePattern,
        Type requestType)
    {
        var endpoint = _app.Services
            .GetRequiredService<EndpointDataSource>()
            .Endpoints
            .OfType<RouteEndpoint>()
            .Single(e =>
                NormalizeRoutePattern(e.RoutePattern.RawText) == routePattern &&
                e.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods.Contains(method) == true);

        var acceptsMetadata = endpoint.Metadata.GetMetadata<IAcceptsMetadata>();

        Assert.NotNull(acceptsMetadata);
        Assert.Equal(requestType, acceptsMetadata.RequestType);
    }

    [Theory]
    [InlineData("GET", "/incomes", StatusCodes.Status200OK, null)]
    [InlineData("POST", "/incomes", StatusCodes.Status200OK, null)]
    [InlineData("GET", "/incomes/{id:guid}", StatusCodes.Status200OK, null)]
    [InlineData("PUT", "/incomes/{id:guid}", StatusCodes.Status200OK, null)]
    [InlineData("DELETE", "/incomes/{id:guid}", StatusCodes.Status200OK, null)]
    [InlineData("GET", "/incomes/categories", StatusCodes.Status200OK, typeof(GetCategoriesResponse))]
    [InlineData("POST", "/incomes/categories", StatusCodes.Status201Created, null)]
    [InlineData("PUT", "/incomes/categories/{id:guid}", StatusCodes.Status200OK, null)]
    [InlineData("DELETE", "/incomes/categories/{id:guid}", StatusCodes.Status200OK, null)]
    [InlineData("POST", "/incomes/categories/{id:guid}/subcategories", StatusCodes.Status201Created, null)]
    [InlineData("DELETE", "/incomes/categories/{id:guid}/subcategories/{subId:guid}", StatusCodes.Status200OK, null)]
    public void Endpoints_document_openapi_response_contracts(
        string method,
        string routePattern,
        int statusCode,
        Type? responseType)
    {
        var endpoint = FindEndpoint(method, routePattern);

        var responseMetadata = endpoint.Metadata
            .GetOrderedMetadata<IProducesResponseTypeMetadata>()
            .SingleOrDefault(metadata => metadata.StatusCode == statusCode);

        Assert.NotNull(responseMetadata);

        if (responseType is not null)
        {
            Assert.Equal(responseType, responseMetadata.Type);
        }
    }

    private static string NormalizeRoutePattern(string? routePattern)
    {
        if (string.IsNullOrWhiteSpace(routePattern))
        {
            return "/";
        }

        var normalized = routePattern.StartsWith('/') ? routePattern : $"/{routePattern}";
        return normalized.Length > 1 ? normalized.TrimEnd('/') : normalized;
    }

    private static HttpRequestMessage CreateRequest(EndpointCase endpointCase)
    {
        var request = new HttpRequestMessage(endpointCase.Method, endpointCase.Path);

        if (endpointCase.Body is not null)
        {
            request.Content = new StringContent(endpointCase.Body, Encoding.UTF8, "application/json");
        }

        return request;
    }

    private RouteEndpoint FindEndpoint(string method, string routePattern)
    {
        return _app.Services
            .GetRequiredService<EndpointDataSource>()
            .Endpoints
            .OfType<RouteEndpoint>()
            .Single(e =>
                NormalizeRoutePattern(e.RoutePattern.RawText) == routePattern &&
                e.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods.Contains(method) == true);
    }

    private sealed record EndpointCase(
        HttpMethod Method,
        string Path,
        string? Body = null,
        HttpStatusCode ExpectedStatus = HttpStatusCode.OK);

    private sealed class TestCurrentUser(Guid userId) : ICurrentUser
    {
        public Guid UserId => userId;
        public string Email => "test-user@wisave.local";
    }

    private sealed class CapturingMessageBus : IMessageBus
    {
        public GetCategoriesResponse CategoriesResponse { get; set; } = new([]);

        public object? Invoked { get; private set; }

        public DeliveryOptions? InvokeOptions { get; private set; }

        public object? Sent { get; private set; }

        public object? Published { get; private set; }

        public string? TenantId { get; set; }

        public Task InvokeAsync(
            object message,
            CancellationToken cancellation = default,
            TimeSpan? timeout = null)
        {
            return Task.CompletedTask;
        }

        public Task InvokeAsync(
            object message,
            DeliveryOptions options,
            CancellationToken cancellation = default,
            TimeSpan? timeout = null)
        {
            return Task.CompletedTask;
        }

        public Task<T> InvokeAsync<T>(
            object message,
            CancellationToken cancellation = default,
            TimeSpan? timeout = null)
        {
            return Task.FromResult(default(T)!);
        }

        public Task<T> InvokeAsync<T>(
            object message,
            DeliveryOptions options,
            CancellationToken cancellation = default,
            TimeSpan? timeout = null)
        {
            Invoked = message;
            InvokeOptions = options;

            if (typeof(T) == typeof(GetCategoriesResponse))
            {
                return Task.FromResult((T)(object)CategoriesResponse);
            }

            return Task.FromResult(default(T)!);
        }

        public async IAsyncEnumerable<TResponse> StreamAsync<TResponse>(
            object message,
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            await Task.CompletedTask;
            yield break;
        }

        public async IAsyncEnumerable<TResponse> StreamAsync<TResponse>(
            object message,
            DeliveryOptions options,
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            await Task.CompletedTask;
            yield break;
        }

        public Task InvokeForTenantAsync(
            string tenantId,
            object message,
            CancellationToken cancellation = default,
            TimeSpan? timeout = null)
        {
            return Task.CompletedTask;
        }

        public Task<T> InvokeForTenantAsync<T>(
            string tenantId,
            object message,
            CancellationToken cancellation = default,
            TimeSpan? timeout = null)
        {
            return Task.FromResult(default(T)!);
        }

        public IDestinationEndpoint EndpointFor(string endpointName)
        {
            throw new NotSupportedException();
        }

        public IDestinationEndpoint EndpointFor(Uri uri)
        {
            throw new NotSupportedException();
        }

        public IReadOnlyList<Envelope> PreviewSubscriptions(object message)
        {
            return [];
        }

        public IReadOnlyList<Envelope> PreviewSubscriptions(object message, DeliveryOptions options)
        {
            return [];
        }

        public ValueTask SendAsync<T>(T message, DeliveryOptions? options = null)
        {
            Sent = message;
            return ValueTask.CompletedTask;
        }

        public ValueTask PublishAsync<T>(T message, DeliveryOptions? options = null)
        {
            Published = message;
            return ValueTask.CompletedTask;
        }

        public ValueTask BroadcastToTopicAsync(string topicName, object message, DeliveryOptions? options = null)
        {
            return ValueTask.CompletedTask;
        }
    }
}
