using AzureFunctions_TextAnalyzer.Functions;
using AzureFunctions_TextAnalyzer.Functions.Dto.Mapper;
using AzureFunctions_TextAnalyzer.Functions.Dto;
using AzureFunctions_TextAnalyzer.Service;
using AzureFunctions_TextAnalyzer.Service.Model;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

builder.Services.AddTransient<IChunkServices>(sp =>
{
    long chunkSize = builder.Configuration.GetValue<long>(Literals.ChunkSize);
    int overlapSize = builder.Configuration.GetValue<int>(Literals.OverlapSize);

    return new ChunkServices(chunkSize, overlapSize);
});

builder.Services.AddSingleton<IDtoMapper<ChunkDataModel, ChunkQueueMessageDto>, ChunkDtoMapper>();

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();
