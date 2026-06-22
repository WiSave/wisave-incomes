using WiSave.Incomes.Core.Application.Incomes.CommandHandlers;
using WiSave.Incomes.Core.Infrastructure;
using WiSave.Incomes.Core.Infrastructure.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddIncomesInfrastructure(
    builder.Configuration.GetConnectionString("General")!,
    builder.Configuration.GetConnectionString("EventStore")!
);
builder.Services.AddIncomesEventPublishing();

builder.AddIncomesWolverine(options => options.IncludeHandlerAssembly<CreateIncomeCommandHandler>());

var host = builder.Build();
host.Run();
