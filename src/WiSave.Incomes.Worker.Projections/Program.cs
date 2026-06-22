using Microsoft.EntityFrameworkCore;
using WiSave.Incomes.Core.Infrastructure.Messaging;
using WiSave.Incomes.Projections;
using WiSave.Incomes.Worker.Projections.Handlers;

var builder = Host.CreateApplicationBuilder(args);

var postgresCs = builder.Configuration.GetConnectionString("Projections")!;

builder.Services.AddDbContext<ProjectionsDbContext>(opts => opts.UseNpgsql(postgresCs));

builder.AddIncomesWolverine(options => options.IncludeHandlerAssembly<IncomeCreatedEventHandler>());

var host = builder.Build();
host.Run();
