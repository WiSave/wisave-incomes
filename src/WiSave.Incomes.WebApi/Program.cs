using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WiSave.Incomes.Core.Infrastructure.Identity;
using WiSave.Incomes.Core.Infrastructure.Messaging;
using WiSave.Incomes.Core.Infrastructure.Postgres;
using WiSave.Incomes.WebApi.Authorization;
using WiSave.Incomes.WebApi.Endpoints;
using WiSave.Incomes.WebApi.Json;
using WiSave.Incomes.WebApi.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IncomesDbContext>(opts => opts.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")!));

builder.Services
    .AddHttpContextAccessor()
    .AddScoped<DevelopmentApiKeyAuthentication>()
    .AddScoped<ICurrentUser, HeaderOrDevelopmentCurrentUser>()
    .AddScoped<PermissionContext>()
    .AddJson()
    .AddOpenApi(options => { options.AddDocumentTransformer<DevelopmentApiKeySecurityTransformer>(); } );

builder.AddIncomesWolverine();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("WiSave Incomes API")
            .WithClassicLayout()
            .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl)
            .AddPreferredSecuritySchemes([DevelopmentApiKeySecurityTransformer.SchemeName])
            .AddApiKeyAuthentication(DevelopmentApiKeySecurityTransformer.SchemeName, apiKey =>
            {
                apiKey.Name = DevelopmentApiKeyAuthentication.HeaderName;
                apiKey.Value = app.Configuration["DevelopmentApiKey:Key"];
            })
            .EnablePersistentAuthentication()
            .ExpandAllTags();
    });
}

app.MapWebApiEndpoints();

app.Run();