using Microsoft.EntityFrameworkCore;
using WiSave.Incomes.Core.Infrastructure.Messaging;
using WiSave.Incomes.Projections.EventHandlers;
using WiSave.Incomes.Projections.Postgres;

var builder = Host.CreateApplicationBuilder(args);

var postgresCs = builder.Configuration.GetConnectionString("Projections")!;

builder.Services.AddDbContext<ProjectionsDbContext>(opts => opts.UseNpgsql(postgresCs));

builder.AddIncomesWolverine(options =>
{
    options.CodeGeneration.AlwaysUseServiceLocationFor<DbContextOptions<ProjectionsDbContext>>();
    options.IncludeHandlerAssembly<IncomeCreatedEventHandler>();
});

var host = builder.Build();
host.Run();
