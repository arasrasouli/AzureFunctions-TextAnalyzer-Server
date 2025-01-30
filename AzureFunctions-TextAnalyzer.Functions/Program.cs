using AzureFunctions_TextAnalyzer.Functions.Dto.Mapper;
using AzureFunctions_TextAnalyzer.Functions.Dto;
using AzureFunctions_TextAnalyzer.Service;
using AzureFunctions_TextAnalyzer.Service.Model;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddScoped<IChunkServices, ChunkServices>();
builder.Services.AddSingleton<IDtoMapper<ChunkDataModel, ChunkQueueMessageDto>, ChunkDtoMapper>();

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();
