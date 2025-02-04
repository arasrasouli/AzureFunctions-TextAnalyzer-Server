using AzureFunctions_TextAnalyzer.Functions;
using AzureFunctions_TextAnalyzer.Functions.Dto.Mapper;
using AzureFunctions_TextAnalyzer.Functions.Dto;
using AzureFunctions_TextAnalyzer.Common;
using AzureFunctions_TextAnalyzer.Service;
using AzureFunctions_TextAnalyzer.Service.Model;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AzureFunctions_TextAnalyzer.DAL.Repositories;
using AzureFunctions_TextAnalyzer.Dto.Mapper;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

builder.Services.AddSingleton<IFileServices>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<FileServices>>();

    var connectionString = builder.Configuration[Literals.AzureWebJobsStorage];
    var containerName = builder.Configuration[Literals.BlobContainer];

    return new FileServices(logger, connectionString, containerName);
});

builder.Services.AddScoped<IChunkServices>(sp =>
{
    var fileServices = sp.GetRequiredService<IFileServices>();

    var chunkWordRepository = sp.GetRequiredService<IChunkWordRepository>();

    var fileChunkRepository = sp.GetRequiredService<IFileChunkRepository>();

    long chunkSize = builder.Configuration.GetValue<long>(Literals.ChunkSize);
    int overlapSize = builder.Configuration.GetValue<int>(Literals.OverlapSize);

    return new ChunkServices(chunkWordRepository, fileChunkRepository, fileServices, chunkSize, overlapSize);
});

builder.Services.AddScoped(typeof(ITableStorageRepository<>), typeof(TableStorageRepository<>));

builder.Services.AddScoped<IChunkWordRepository>(sp =>
{
    string connectionString = builder.Configuration.GetValue<string>(Literals.TableStorageConnectionString);
    string tableName = builder.Configuration.GetValue<string>(Literals.ChunkTableName);
    return new ChunkWordRepository(connectionString, tableName);
});

builder.Services.AddScoped<IFileChunkRepository>(sp =>
{
    string connectionString = builder.Configuration.GetValue<string>(Literals.TableStorageConnectionString);
    string tableName = builder.Configuration.GetValue<string>(Literals.FileTableName);
    return new FileChunkRepository(connectionString, tableName);
});

builder.Services.AddSingleton<IMapperFactory, MapperFactory>();
builder.Services.AddSingleton<IDtoMapper<ChunkDataModel, ChunkQueueMessageDto>, ChunkDtoMapper>();
builder.Services.AddSingleton<IDtoMapper<FileChunkModel, FileChunkDto>, FileChunkDtoMapper>();

builder.ConfigureFunctionsWebApplication();

builder.Build().Run();