using AzureFunctions_TextAnalyzer.Service;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddScoped<IChunkServices, ChunkServices>();

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();
